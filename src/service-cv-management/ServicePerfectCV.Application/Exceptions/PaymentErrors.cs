using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class PaymentErrors
    {
        public static readonly Error CreatePaymentFailed = new(
            Code: "CreatePaymentFailed",
            Message: "Failed to create payment link.",
            HttpStatusCode.BadRequest);

        public static readonly Error PaymentInfoNotFound = new(
            Code: "PaymentInfoNotFound",
            Message: "Payment information not found.",
            HttpStatusCode.NotFound);

        public static readonly Error CancelPaymentFailed = new(
            Code: "CancelPaymentFailed",
            Message: "Failed to cancel payment.",
            HttpStatusCode.BadRequest);

        public static readonly Error WebhookProcessingFailed = new(
            Code: "WebhookProcessingFailed",
            Message: "Failed to process payment webhook.",
            HttpStatusCode.BadRequest);

        public static readonly Error InvalidOrderCode = new(
            Code: "InvalidOrderCode",
            Message: "Invalid order code provided.",
            HttpStatusCode.BadRequest);

        public static readonly Error PaymentAlreadyCancelled = new(
            Code: "PaymentAlreadyCancelled",
            Message: "Payment has already been cancelled.",
            HttpStatusCode.Conflict);

        public static readonly Error PaymentConfigurationError = new(
            Code: "PaymentConfigurationError",
            Message: "Payment service configuration error.",
            HttpStatusCode.InternalServerError);

        public static readonly Error PackageNotFound = new(
            Code: "PackageNotFound",
            Message: "Package not found for payment.",
            HttpStatusCode.NotFound);

        public static readonly Error PackageInactive = new(
            Code: "PackageInactive",
            Message: "Package is not active for purchase.",
            HttpStatusCode.BadRequest);

        public static readonly Error AmountMismatch = new(
            Code: "AmountMismatch",
            Message: "Request amount does not match package price.",
            HttpStatusCode.BadRequest);

        public static readonly Error InvalidPaymentRequest = new(
            Code: "InvalidPaymentRequest",
            Message: "Invalid payment request data.",
            HttpStatusCode.BadRequest);

        public static readonly Error PaymentServiceUnavailable = new(
            Code: "PaymentServiceUnavailable",
            Message: "Payment service is temporarily unavailable.",
            HttpStatusCode.ServiceUnavailable);
    }
}