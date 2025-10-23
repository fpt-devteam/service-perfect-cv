namespace ServicePerfectCV.WebAdmin.Models.CVs
{
    public class CVListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public bool IsStructuredDone { get; set; }
        public bool HasPdf { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int EducationCount { get; set; }
        public int ExperienceCount { get; set; }
        public int SkillCount { get; set; }
        public int ProjectCount { get; set; }
        public int CertificationCount { get; set; }
    }
}

