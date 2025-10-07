using System;

namespace ServicePerfectCV.Application.DTOs.Billing.Requests
{
    public class CreateBillingHistoryRequest
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public decimal Amount { get; set; }
        public string GatewayTransactionId { get; set; } = default!;
    }
}