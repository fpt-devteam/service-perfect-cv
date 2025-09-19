using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.Entities
{
    public class EmploymentType : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation property
        public virtual ICollection<Experience> Experiences { get; set; } = [];
    }
}