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
        
        public Guid? JobTitleId { get; set; }
        public required string JobTitle { get; set; }
        
        public required Guid EmploymentTypeId { get; set; }
        
        public Guid? OrganizationId { get; set; }
        public required string Organization { get; set; }
        
        public string? Location { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual CV CV { get; set; } = default!;
        public virtual JobTitle? JobTitleNavigation { get; set; }
        public virtual EmploymentType EmploymentType { get; set; } = default!;
        public virtual Organization? OrganizationNavigation { get; set; }
    }
}