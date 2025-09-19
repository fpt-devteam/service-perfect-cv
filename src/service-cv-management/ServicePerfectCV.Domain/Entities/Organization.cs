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
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

    }
}