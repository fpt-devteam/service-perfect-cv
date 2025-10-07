using ServicePerfectCV.Domain.Enums;
using System;

namespace ServicePerfectCV.Application.DTOs.Billing.Requests
{
    public class CreateBillingHistoryRequest
    {
        public required Guid UserId { get; set; }
        public required Guid PackageId { get; set; }
        public required decimal Amount { get; set; }
        public required PaymentStatus Status { get; set; }
        public required string GatewayTransactionId { get; set; }
    }
}