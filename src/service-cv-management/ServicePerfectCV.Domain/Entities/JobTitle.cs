using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.Entities
{
    public class JobTitle : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation property
        public virtual ICollection<Experience> Experiences { get; set; } = [];
    }
}
