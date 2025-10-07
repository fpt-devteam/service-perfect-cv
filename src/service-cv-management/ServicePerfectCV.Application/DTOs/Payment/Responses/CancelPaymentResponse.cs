using System;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Payment.Responses
{
    public class CancelPaymentResponse
    {
        public required int OrderCode { get; init; }
        public required PaymentStatus Status { get; init; }
        public DateTimeOffset CancelledAt { get; init; } = DateTimeOffset.UtcNow;
        public string? CancellationReason { get; init; }
    }
}