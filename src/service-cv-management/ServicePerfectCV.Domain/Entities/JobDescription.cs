using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.ValueObjects;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class JobDescription : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }
        public required string Title { get; set; }
        public required string CompanyName { get; set; }
        public required string Responsibility { get; set; }
        public required string Qualification { get; set; }
        public SectionRubricDictionary? SectionRubrics { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation property
        public virtual CV CV { get; set; } = default!;
    }
}