using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Payment.Requests;
using ServicePerfectCV.Application.DTOs.Payment.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly PaymentUrlSettings _paymentUrlSettings;

        public PaymentController(PaymentService paymentService, IOptions<PaymentUrlSettings> paymentUrlSettings)
        {
            _paymentService = paymentService;
            _paymentUrlSettings = paymentUrlSettings.Value;
        }

        /// <summary>
        /// Create a new payment link using PayOS API
        /// </summary>
        /// <param name="request">Payment creation request with package ID</param>
        /// <returns>Payment link information including checkout URL and QR code</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/payment/create-payment
        ///     {
        ///         "packageId": "57e2340a-c5fe-45bc-9ba5-558bfacd7561",
        ///     }
        /// 
        /// </remarks>
        [Authorize]
        [HttpPost("create-payment")]
        [ProducesResponseType(typeof(CreatePaymentResponse), 200)]
        [ProducesResponseType(typeof(object), 400, "application/json")] // InvalidPaymentRequest, AmountMismatch, PackageInactive
        [ProducesResponseType(typeof(object), 404, "application/json")] // PackageNotFound
        [ProducesResponseType(typeof(object), 500, "application/json")] // CreatePaymentFailed, PaymentConfigurationError
        [ProducesResponseType(typeof(object), 503, "application/json")] // PaymentServiceUnavailable
        public async Task<ActionResult<CreatePaymentResponse>> CreatePaymentLink([FromBody] CreatePaymentRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(PaymentErrors.InvalidPaymentRequest);

            // ModelState validation will be handled by middleware if needed
            var response = await _paymentService.CreatePaymentLinkAsync(request, userId);
            return Ok(response);
        }

        /// <summary>
        /// Get payment information by order code
        /// </summary>
        /// <param name="orderCode">The order code to lookup</param>
        /// <returns>Payment information</returns>
        [Authorize]
        [HttpGet("payment-info/{orderCode:int}")]
        [ProducesResponseType(typeof(PaymentInfoResponse), 200)]
        [ProducesResponseType(typeof(object), 400, "application/json")] // InvalidOrderCode
        [ProducesResponseType(typeof(object), 404, "application/json")] // PaymentInfoNotFound
        [ProducesResponseType(typeof(object), 500, "application/json")] // PaymentServiceUnavailable
        public async Task<ActionResult<PaymentInfoResponse>> GetPaymentInfo([FromRoute] int orderCode)
        {
            if (orderCode <= 0)
            {
                throw new DomainException(PaymentErrors.InvalidOrderCode);
            }

            var response = await _paymentService.GetPaymentInfoAsync(orderCode);
            return Ok(response);
        }

        /// <summary>
        /// Cancel a payment by order code
        /// </summary>
        /// <param name="orderCode">The order code to cancel</param>
        /// <param name="request">Cancellation request details</param>
        /// <returns>Cancellation result</returns>
        [Authorize]
        [HttpPost("payment/{orderCode:int}/cancel")]
        [ProducesResponseType(typeof(CancelPaymentResponse), 200)]
        [ProducesResponseType(typeof(object), 400, "application/json")] // InvalidOrderCode, CancelPaymentFailed
        [ProducesResponseType(typeof(object), 404, "application/json")] // PaymentInfoNotFound
        [ProducesResponseType(typeof(object), 409, "application/json")] // PaymentAlreadyCancelled
        [ProducesResponseType(typeof(object), 500, "application/json")] // PaymentServiceUnavailable
        public async Task<ActionResult<CancelPaymentResponse>> CancelPayment(
                [FromRoute] int orderCode,
                [FromBody] CancelPaymentRequest request)
        {
            if (orderCode <= 0)
            {
                throw new DomainException(PaymentErrors.InvalidOrderCode);
            }

            var response = await _paymentService.CancelPaymentAsync(orderCode, request);
            return Ok(response);
        }

        /// <summary>
        /// Process payment webhook from PayOS
        /// </summary>
        /// <param name="webhookData">Webhook data from PayOS</param>
        /// <returns>Webhook processing result</returns>

        [HttpPost("receive-hook")]
        [ProducesResponseType(typeof(WebhookResponse), 200)]
        [ProducesResponseType(typeof(object), 400, "application/json")] // WebhookProcessingFailed
        [ProducesResponseType(typeof(object), 500, "application/json")] // PaymentServiceUnavailable
        public async Task<ActionResult<WebhookResponse>> ProcessWebhook([FromBody] object webhookData)
        {
            if (webhookData == null)
            {
                throw new DomainException(PaymentErrors.WebhookProcessingFailed);
            }

            var response = await _paymentService.ProcessWebhookAsync(webhookData);
            return Ok(webhookData);
        }

        /// <summary>
        /// Get payment status by order code
        /// </summary>
        /// <param name="orderCode">The order code to check status</param>
        /// <returns>Payment status summary</returns>
        [Authorize]
        [HttpGet("status/{orderCode:int}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400, "application/json")] // InvalidOrderCode
        [ProducesResponseType(typeof(object), 404, "application/json")] // PaymentInfoNotFound
        [ProducesResponseType(typeof(object), 500, "application/json")] // PaymentServiceUnavailable
        public async Task<ActionResult> GetPaymentStatus([FromRoute] int orderCode)
        {
            if (orderCode <= 0)
            {
                throw new DomainException(PaymentErrors.InvalidOrderCode);
            }

            var paymentInfo = await _paymentService.GetPaymentInfoAsync(orderCode);

            return Ok(new
            {
                OrderCode = paymentInfo.OrderCode,
                Status = paymentInfo.Status,
                Amount = paymentInfo.Amount,
                CreatedAt = paymentInfo.CreatedAt,
                TransactionDateTime = paymentInfo.TransactionDateTime
            });
        }

        /// <summary>
        /// Handle successful payment return from PayOS
        /// This endpoint is called by PayOS when payment is successful
        /// </summary>
        /// <param name="orderCode">Order code from PayOS</param>
        /// <param name="status">Payment status from PayOS</param>
        /// <param name="cancel">Cancel indicator from PayOS</param>
        /// <returns>Redirect to frontend success page</returns>
        // [HttpGet("success")]
        // public async Task<IActionResult> HandlePaymentSuccess(
        //     [FromQuery] int? orderCode,
        //     [FromQuery] string? status,
        //     [FromQuery] bool? cancel = false)
        // {
        //     // Delegate business logic to PaymentService which returns the frontend redirect URL
        //     var result = await _paymentService.HandleReturnCallbackAsync(orderCode, status, cancel);
        //     if (result.Success)
        //     {
        //         return Ok(new { message = "Processed", response = result.Response });
        //     }

        //     return StatusCode(500, new { message = "Failed to process callback", error = result.Response });
        // }

        /// <summary>
        /// Handle cancelled payment return from PayOS
        /// This endpoint is called by PayOS when payment is cancelled
        /// </summary>
        /// <param name="orderCode">Order code from PayOS</param>
        /// <param name="status">Payment status from PayOS</param>
        /// <param name="cancel">Cancel indicator from PayOS</param>
        /// <returns>Redirect to frontend cancel page</returns>
        // [HttpGet("cancel")]
        // public async Task<IActionResult> HandlePaymentCancel(
        //     [FromQuery] int? orderCode,
        //     [FromQuery] string? status,
        //     [FromQuery] bool? cancel = true)
        // {
        //     var result = await _paymentService.HandleCancelCallbackAsync(orderCode, status, cancel);
        //     if (result.Success)
        //     {
        //         return Ok(new { message = "Processed", response = result.Response });
        //     }

        //     return StatusCode(500, new { message = "Failed to process callback", error = result.Response });
        // }

        /// <summary>
        /// Get payment callback URLs (for testing/debugging purposes)
        /// </summary>
        /// <returns>The configured callback URLs</returns>
        [HttpGet("callback-urls")]
        public IActionResult GetCallbackUrls()
        {
            return Ok(new
            {
                ReturnUrl = _paymentUrlSettings.GetReturnUrl(),
                CancelUrl = _paymentUrlSettings.GetCancelUrl(),
                FrontendSuccessUrl = _paymentUrlSettings.FrontendSuccessUrl,
                FrontendCancelUrl = _paymentUrlSettings.FrontendCancelUrl
            });
        }
    }
}