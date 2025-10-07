using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.Entities
{
    public class Package : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int NumCredits { get; set; }
        public required bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation properties
        public ICollection<BillingHistory> BillingHistories { get; set; } = [];
    }
}