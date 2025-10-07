using ServicePerfectCV.Domain.Enums;
using System;

namespace ServicePerfectCV.Application.DTOs.Billing.Responses
{
    public class BillingHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string GatewayTransactionId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}