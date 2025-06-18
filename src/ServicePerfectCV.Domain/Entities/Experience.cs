using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Experience : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        
        // Job Title
        public Guid? JobTitleId { get; set; }
        public string? JobTitle { get; set; }
        
        // Employment Type
        public Guid EmploymentTypeId { get; set; }
        
        // Company
        public Guid? CompanyId { get; set; }
        public string? Company { get; set; }
        
        public string? Location { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual CV Cv { get; set; } = default!;
        public virtual JobTitle? JobTitleNavigation { get; set; }
        public virtual EmploymentType EmploymentType { get; set; } = default!;
        public virtual Company? CompanyNavigation { get; set; }
    }
}