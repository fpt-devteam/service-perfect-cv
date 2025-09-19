using ServicePerfectCV.Domain.Common;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class Education : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }

        public required string Degree { get; set; }

        public required string Organization { get; set; }

        public string? FieldOfStudy { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? Gpa { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public virtual CV CV { get; set; } = default!;
    }
}