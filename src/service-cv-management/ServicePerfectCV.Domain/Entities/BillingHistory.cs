using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class BillingHistory : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid PackageId { get; set; }
        public required decimal Amount { get; set; }
        public required PaymentStatus Status { get; set; }
        public string? GatewayTransactionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Package Package { get; set; } = null!;
    }
}