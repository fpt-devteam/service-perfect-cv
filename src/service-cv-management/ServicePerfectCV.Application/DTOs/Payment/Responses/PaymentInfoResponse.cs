using System;
using System.Collections.Generic;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Payment.Responses
{
    public class PaymentInfoResponse
    {
        public required int OrderCode { get; init; }
        public required int Amount { get; init; }
        public required PaymentStatus Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? TransactionDateTime { get; init; }
        public List<PaymentItemResponse> Items { get; init; } = new();
    }
}