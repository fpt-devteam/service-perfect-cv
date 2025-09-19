using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Certification : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }
        public required string Name { get; set; }
        public required string Organization { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation property
        public virtual CV CV { get; set; } = default!;
    }
}