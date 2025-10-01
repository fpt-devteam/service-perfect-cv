using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class SectionScoreResult : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        public SectionType SectionType { get; set; }
        public string JdHash { get; set; } = default!;
        public string SectionContentHash { get; set; } = default!;
        public SectionScore SectionScore { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation property
        public virtual CV CV { get; set; } = default!;
    }
}