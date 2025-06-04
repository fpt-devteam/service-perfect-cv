using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class CVS : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid TemplateId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? SummaryId { get; set; }
        public required string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public required User User { get; set; }
        public required Template Template { get; set; } = null!;
        public Contact Contact { get; set; } = default!;
        public Summary Summary { get; set; } = default!;
        public ICollection<Education> Educations { get; set; } = [];
        public ICollection<Experience> Experiences { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<Skill> Skills { get; set; } = [];
        public ICollection<Certification> Certifications { get; set; } = [];
    }
}