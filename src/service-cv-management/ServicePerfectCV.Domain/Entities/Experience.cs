using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Experience : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }

        public required string JobTitle { get; set; }

        public required Guid EmploymentTypeId { get; set; }

        public required string Organization { get; set; }

        public string? Location { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public virtual CV CV { get; set; } = default!;
        public virtual EmploymentType EmploymentType { get; set; } = default!;
    }
}