using System;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Payment.Responses
{
    public class CreatePaymentResponse
    {
        public required int OrderCode { get; init; }
        public required string CheckoutUrl { get; init; }
        public required string QrCode { get; init; }
        public required PaymentStatus Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}