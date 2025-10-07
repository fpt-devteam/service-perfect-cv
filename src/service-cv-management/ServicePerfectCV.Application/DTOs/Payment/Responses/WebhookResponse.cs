using System;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Payment.Responses
{
    public class WebhookResponse
    {
        public required PaymentStatus Status { get; init; }
        public required string Message { get; init; }
        public WebhookDataResponse? Data { get; init; }
    }

    public class WebhookDataResponse
    {
        public required int OrderCode { get; init; }
        public required int Amount { get; init; }
        public required string Description { get; init; }
        public string? AccountNumber { get; init; }
        public string? Reference { get; init; }
        public DateTimeOffset TransactionDateTime { get; init; }
    }
}