using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class SectionScoreResult : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }
        public required SectionType SectionType { get; set; }
        public required string JdHash { get; set; }
        public required string SectionContentHash { get; set; }
        public required SectionScore SectionScore { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation property
        public virtual CV CV { get; set; } = default!;
    }
}