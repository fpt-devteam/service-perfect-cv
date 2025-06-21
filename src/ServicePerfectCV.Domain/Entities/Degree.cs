using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.Entities
{
    public class Degree : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation property
        public virtual ICollection<Education> Educations { get; set; } = [];
    }
}
