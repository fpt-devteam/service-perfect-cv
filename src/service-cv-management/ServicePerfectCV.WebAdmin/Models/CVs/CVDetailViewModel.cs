using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.DTOs.Summary.Responses;

namespace ServicePerfectCV.WebAdmin.Models.CVs
{
    public class CVDetailViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public Guid? VersionId { get; set; }
        public bool IsStructuredDone { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // PDF info
        public string? PdfFileName { get; set; }
        public string? ExtractedText { get; set; }

        // CV Content
        public ContactResponse? Contact { get; set; }
        public SummaryResponse? Summary { get; set; }
        public List<EducationResponse> Educations { get; set; } = new();
        public List<ExperienceResponse> Experiences { get; set; } = new();
        public List<ProjectResponse> Projects { get; set; } = new();
        public List<SkillResponse> Skills { get; set; } = new();
        public List<CertificationResponse> Certifications { get; set; } = new();
    }
}

