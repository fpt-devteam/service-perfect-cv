using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class CV : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? VersionId { get; set; }
        public Guid? AnalysisId { get; set; }
        public string Title { get; set; } = default!;
        public CVContent? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = default!;
        public JobDescription? JobDescription { get; set; }
        public Contact Contact { get; set; } = default!;
        public Summary Summary { get; set; } = default!;
        public ICollection<Education> Educations { get; set; } = [];
        public ICollection<Experience> Experiences { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<Skill> Skills { get; set; } = [];
        public ICollection<Certification> Certifications { get; set; } = [];
    }
}