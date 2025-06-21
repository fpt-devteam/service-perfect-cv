using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.Entities
{
    public class Organization : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public required OrganizationType OrganizationType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Experience> Experiences { get; set; } = [];
    }
}
