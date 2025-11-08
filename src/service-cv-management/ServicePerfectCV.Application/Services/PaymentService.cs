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
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Interfaces.Repositories;

namespace ServicePerfectCV.Application.Services
{
    public class PaymentService
    {
        private readonly PayOS _payOS;
        private readonly IPackageRepository _packageRepository;
        private readonly PaymentUrlSettings _paymentUrlSettings;
        private readonly BillingHistoryService _billingHistoryService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IUserRepository _userRepository;

        public PaymentService(
            PayOS payOS,
            IPackageRepository packageRepository,
            IOptions<PaymentUrlSettings> paymentUrlSettings,
            ILogger<PaymentService> logger,
            BillingHistoryService billingHistoryService,
            IUserRepository userRepository
              )
        {
            _payOS = payOS;
            _packageRepository = packageRepository;
            _paymentUrlSettings = paymentUrlSettings.Value;
            _billingHistoryService = billingHistoryService;
            _logger = logger;
            _userRepository = userRepository;
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

            // Create billing history record with Pending status
            await _billingHistoryService.CreateAsync(
                new CreateBillingHistoryRequest
                {
                    UserId = userId,
                    PackageId = request.PackageId,
                    Amount = package.Price,
                    Status = PaymentStatus.Pending,
                    GatewayTransactionId = orderCode.ToString()
                }
            );

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

            // Map to response DTO
            return new CreatePaymentResponse
            {
                OrderCode = orderCode,
                CheckoutUrl = createPayment.checkoutUrl,
                QrCode = createPayment.qrCode
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

        public async Task<(bool Success, string? Response)> HandlePaymentSuccessAsync(long orderCode)
        {
            try
            {
                // Find existing billing history by GatewayTransactionId (which stores the orderCode)
                var billingHistory = await _billingHistoryService.GetByGatewayTransactionIdAsync(orderCode.ToString());

                if (billingHistory == null)
                {
                    return (false, $"Billing history not found for order code: {orderCode}");
                }

                // Update billing history status to Success
                var billingUpdate = await _billingHistoryService.UpdateAsync(billingHistory.Id, new UpdateBillingHistoryRequest
                {
                    Status = PaymentStatus.Success
                });

                // Get package details to add credits to user
                var package = await _packageRepository.GetByIdAsync(billingHistory.PackageId);
                if (package != null)
                {
                    // Get user and update their total credits
                    var user = await _userRepository.GetByIdAsync(billingHistory.UserId);
                    if (user != null)
                    {
                        user.TotalCredit += package.NumCredits;
                        user.UpdatedAt = DateTimeOffset.UtcNow;

                        _userRepository.Update(user);
                        await _userRepository.SaveChangesAsync();

                        _logger.LogInformation(
                            "Added {Credits} credits to user {UserId}. New total: {TotalCredits}",
                            package.NumCredits, user.Id, user.TotalCredit);
                    }
                    else
                    {
                        _logger.LogWarning("User {UserId} not found for credit update", billingHistory.UserId);
                    }
                }

                return (true, "Payment processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment success for order code: {OrderCode}", orderCode);
                return (false, $"Error processing payment success: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Response)> HandlePaymentFailAsync(long orderCode)
        {
            try
            {

                // Find existing billing history by GatewayTransactionId (which stores the orderCode)
                var billingHistory = await _billingHistoryService.GetByGatewayTransactionIdAsync(orderCode.ToString());

                if (billingHistory == null)
                {
                    return (false, $"Billing history not found for order code: {orderCode}");
                }

                // Update billing history status to Failed
                await _billingHistoryService.UpdateAsync(billingHistory.Id, new UpdateBillingHistoryRequest
                {
                    Status = PaymentStatus.Failed
                });

                return (true, "Payment failure recorded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment failure for order code: {OrderCode}", orderCode);
                return (false, $"Error processing payment failure: {ex.Message}");
            }
        }

        public async Task<CancelPaymentResponse> CancelPaymentAsync(int orderCode, CancelPaymentRequest request)
        {
            try
            {

                PaymentLinkInformation cancelledPayment = await _payOS.cancelPaymentLink(orderCode, request.CancellationReason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment for order code: {OrderCode}", orderCode);
            }
            await HandlePaymentFailAsync(orderCode);

            return new CancelPaymentResponse
            {
                OrderCode = orderCode,
                CancellationReason = request.CancellationReason
            };
        }

        public async Task<bool> ProcessWebhookAsync(WebhookType webhookData)
        {
            // Verify webhook data
            _logger.LogInformation("Processed webhook: {@Webhook}", webhookData);
            WebhookData verifiedData = _payOS.verifyPaymentWebhookData(webhookData);

            // Parse payment status from PayOS data
            _logger.LogInformation("Verified webhook data: {@VerifiedData}", verifiedData);
            _logger.LogInformation("Payment code: {Code}", verifiedData.code);
            PaymentStatus paymentStatus = ParsePaymentStatus(verifiedData.code);

            if (paymentStatus == PaymentStatus.Failed)
            {
                await HandlePaymentFailAsync(verifiedData.orderCode);
            }
            else if (paymentStatus == PaymentStatus.Success)
            {
                await HandlePaymentSuccessAsync(verifiedData.orderCode);
            }
            return true;
        }

        private static PaymentStatus ParsePaymentStatus(string status)
        {
            return status?.ToLower() switch
            {
                "00" => PaymentStatus.Success,
                "01" => PaymentStatus.Failed,
                _ => PaymentStatus.Failed
            };
        }

        /// <summary>
        /// Generate unique order code using timestamp + random number to avoid collisions
        /// Format: Unix timestamp + Random 3-digit number
        /// Result: 10-digit positive number that fits safely in int32 range (max: ~2.1 billion)
        /// </summary>
        private static int GenerateUniqueOrderCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Giảm xuống 2 triệu (7 chữ số) để tránh overflow khi nhân 1000
            // 2,147,483 * 1000 + 999 = 2,147,483,999 < int.MaxValue (2,147,483,647)
            int timestampPart = (int)(timestamp % 2147483); // An toàn cho int32

            Random random = new Random();
            int randomPart = random.Next(100, 1000); // 3 chữ số (100-999)

            int orderCode = timestampPart * 1000 + randomPart;

            // Double check - đảm bảo 100% dương
            return Math.Abs(orderCode);
        }
    }
}