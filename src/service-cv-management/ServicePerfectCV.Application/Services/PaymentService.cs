using ServicePerfectCV.Application.DTOs.Payment.Requests;
using ServicePerfectCV.Application.DTOs.Payment.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.Entities;
using Net.payOS;
using Net.payOS.Types;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;
using ServicePerfectCV.Application.DTOs.Billing.Requests;

namespace ServicePerfectCV.Application.Services
{
    public class PaymentService
    {
        private readonly PayOS _payOS;
        private readonly IPackageRepository _packageRepository;
        private readonly PaymentUrlSettings _paymentUrlSettings;
        private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;
        private readonly BillingHistoryService _billingHistoryService;

        public PaymentService(
            PayOS payOS, IPackageRepository packageRepository,
            IOptions<PaymentUrlSettings> paymentUrlSettings,
            System.Net.Http.IHttpClientFactory httpClientFactory,
            BillingHistoryService billingHistoryService
              )
        {
            _payOS = payOS;
            _packageRepository = packageRepository;
            _paymentUrlSettings = paymentUrlSettings.Value;
            _httpClientFactory = httpClientFactory;
            _billingHistoryService = billingHistoryService;
        }

        public async Task<CreatePaymentResponse> CreatePaymentLinkAsync(CreatePaymentRequest request, Guid userId)
        {
            int orderCode = GenerateUniqueOrderCode();

            // Get package from database by PackageId
            var package = await _packageRepository.GetByIdAsync(request.PackageId)
                ?? throw new DomainException(PaymentErrors.PackageNotFound);

            var items = new List<ItemData>
            {
                new ItemData(
                    name: package.Name,
                    quantity: 1,
                    price: (int)package.Price // Convert decimal to int for PayOS
                )
            };


            // Build cancel/return URLs by replacing the {orderCode} placeholder
            var cancelUrl = BuildCallbackUrl(_paymentUrlSettings.BaseUrl, _paymentUrlSettings.CancelEndpoint, orderCode);
            var returnUrl = BuildCallbackUrl(_paymentUrlSettings.BaseUrl, _paymentUrlSettings.SuccessEndpoint, orderCode);

            // Create payment data with concrete callback URLs
            PaymentData paymentData = new PaymentData(
                orderCode,
                (int)package.Price,
                package.Name,
                items,
                cancelUrl,
                returnUrl
            );

            // Create payment link via PayOS
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            await _billingHistoryService.CreateAsync(
                new CreateBillingHistoryRequest
                {
                    UserId = userId,
                    PackageId = request.PackageId,
                    Amount = package.Price,
                    GatewayTransactionId = orderCode.ToString()
                }
            );

            // Map to response DTO
            return new CreatePaymentResponse
            {
                OrderCode = orderCode,
                CheckoutUrl = createPayment.checkoutUrl,
                QrCode = createPayment.qrCode,
                Status = PaymentStatus.Pending
            };
        }

        private static string BuildCallbackUrl(string baseUrl, string endpointTemplate, int orderCode)
        {
            if (string.IsNullOrEmpty(baseUrl)) baseUrl = string.Empty;
            if (string.IsNullOrEmpty(endpointTemplate)) endpointTemplate = string.Empty;

            // Replace both constrained and unconstrained placeholders
            var replaced = endpointTemplate
                .Replace("{orderCode:int}", orderCode.ToString())
                .Replace("{orderCode}", orderCode.ToString());

            return $"{baseUrl.TrimEnd('/')}/{replaced.TrimStart('/')}";
        }

        public async Task<PaymentInfoResponse> GetPaymentInfoAsync(int orderCode)
        {
            PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

            return new PaymentInfoResponse
            {
                OrderCode = (int)paymentInfo.orderCode,
                Amount = paymentInfo.amount,
                Status = ParsePaymentStatus(paymentInfo.status),
                CreatedAt = DateTimeOffset.TryParse(paymentInfo.createdAt, out var createdAt)
                    ? createdAt
                    : DateTimeOffset.UtcNow
            };
        }

        public async Task<(bool Success, string? Response)> HandlePaymentSuccessAsync(int? orderCode, string? status, bool? cancel)
        {
            try
            {
                if (!orderCode.HasValue)
                {
                    return (false, "Order code is required");
                }

                // Find billing history by GatewayTransactionId (which stores the orderCode)
                var billingHistory = await _billingHistoryService.GetByGatewayTransactionIdAsync(orderCode.Value.ToString());

                if (billingHistory == null)
                {
                    return (false, $"Billing history not found for order code: {orderCode}");
                }

                // Update billing history status to Completed
                await _billingHistoryService.UpdateAsync(billingHistory.Id, new UpdateBillingHistoryRequest
                {
                    Status = PaymentStatus.Completed
                });

                // Get package details to add credits to user
                var package = await _packageRepository.GetByIdAsync(billingHistory.PackageId);
                if (package != null)
                {
                    // TODO: Implement user credit update logic
                    // This requires IUserRepository to update TotalCredit
                    // await _userRepository.AddCreditsAsync(billingHistory.UserId, package.NumCredits);
                }

                return (true, "Payment processed successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error processing payment success: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Response)> HandlePaymentFailAsync(int? orderCode, string? status, bool? cancel)
        {
            try
            {
                if (!orderCode.HasValue)
                {
                    return (false, "Order code is required");
                }

                // Find billing history by GatewayTransactionId (which stores the orderCode)
                var billingHistory = await _billingHistoryService.GetByGatewayTransactionIdAsync(orderCode.Value.ToString());

                if (billingHistory == null)
                {
                    return (false, $"Billing history not found for order code: {orderCode}");
                }

                // Determine the appropriate failure status
                PaymentStatus failureStatus = cancel == true ? PaymentStatus.Canceled : PaymentStatus.Failed;

                // Update billing history status to Failed or Canceled
                await _billingHistoryService.UpdateAsync(billingHistory.Id, new UpdateBillingHistoryRequest
                {
                    Status = failureStatus
                });

                return (true, $"Payment marked as {failureStatus.ToString().ToLower()}");
            }
            catch (Exception ex)
            {
                return (false, $"Error processing payment failure: {ex.Message}");
            }
        }

        public async Task<CancelPaymentResponse> CancelPaymentAsync(int orderCode, CancelPaymentRequest request)
        {
            PaymentLinkInformation cancelledPayment = await _payOS.cancelPaymentLink(orderCode, request.CancellationReason);

            return new CancelPaymentResponse
            {
                OrderCode = (int)cancelledPayment.orderCode,
                Status = ParsePaymentStatus(cancelledPayment.status),
                CancellationReason = request.CancellationReason
            };
        }

        public Task<WebhookResponse> ProcessWebhookAsync(object webhookData)
        {
            // Verify webhook data
            WebhookData verifiedData = _payOS.verifyPaymentWebhookData((WebhookType)webhookData);

            // Parse payment status from PayOS data
            PaymentStatus paymentStatus = ParsePaymentStatus(verifiedData.code);

            // TODO: Add business logic here based on payment status
            // - Update order status in database
            // - Send notification to user
            // - Update user credits/subscription (only if status is Completed)
            // - Log transaction

            WebhookResponse webhook = new WebhookResponse
            {
                Status = paymentStatus,
                Message = GetWebhookMessage(paymentStatus),
                Data = new WebhookDataResponse
                {
                    OrderCode = (int)verifiedData.orderCode,
                    Amount = verifiedData.amount,
                    Description = verifiedData.description,
                    AccountNumber = verifiedData.accountNumber,
                    Reference = verifiedData.reference,
                    TransactionDateTime = DateTimeOffset.TryParse(verifiedData.transactionDateTime, out var transactionTime)
                        ? transactionTime
                        : DateTimeOffset.UtcNow
                }
            };

            // Log webhook response for debugging
            Console.WriteLine($"Webhook Response: Status={webhook.Status}, Message={webhook.Message}, OrderCode={webhook.Data.OrderCode}");

            return Task.FromResult(webhook);
        }

        private static PaymentStatus ParsePaymentStatus(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => PaymentStatus.Pending,
                "processing" => PaymentStatus.Processing,
                "paid" or "success" or "completed" => PaymentStatus.Completed,
                "failed" or "failure" => PaymentStatus.Failed,
                "cancelled" or "canceled" => PaymentStatus.Canceled,
                "expired" => PaymentStatus.Expired,
                _ => PaymentStatus.Pending
            };
        }

        private static string GetWebhookMessage(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Completed => "Payment completed successfully",
                PaymentStatus.Failed => "Payment failed",
                PaymentStatus.Canceled => "Payment was cancelled",
                PaymentStatus.Expired => "Payment has expired",
                PaymentStatus.Processing => "Payment is being processed",
                PaymentStatus.Pending => "Payment is pending",
                _ => "Webhook processed"
            };
        }

        /// <summary>
        /// Generate unique order code using timestamp + random number to avoid collisions
        /// Format: Unix timestamp (10 digits) + Random 3-digit number
        /// Result: 13-digit unique number that fits in int32 range
        /// </summary>
        private static int GenerateUniqueOrderCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            int timestampPart = (int)(timestamp % 10000000); // 7 digits max

            Random random = new Random();
            int randomPart = random.Next(100, 1000); // 3 digits

            return timestampPart * 1000 + randomPart;
        }
    }
}