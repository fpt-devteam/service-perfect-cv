using ServicePerfectCV.Domain.Common;
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

        // Navigation property
        public virtual CV CV { get; set; } = default!;
    }
}