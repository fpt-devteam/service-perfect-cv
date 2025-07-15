using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class CVFullContentResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; } = default!;
        public Guid? VersionId { get; init; }
        public Guid? AnalysisId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public JobDetailDto? JobDetail { get; init; }
        public ContactResponse? Contact { get; init; }
        public SummaryResponse? Summary { get; init; }
        public IEnumerable<SkillResponse> Skills { get; init; } = new List<SkillResponse>();
        public IEnumerable<EducationResponse> Educations { get; init; } = new List<EducationResponse>();
        public IEnumerable<ExperienceResponse> Experiences { get; init; } = new List<ExperienceResponse>();
        public IEnumerable<ProjectResponse> Projects { get; init; } = new List<ProjectResponse>();
        public IEnumerable<CertificationResponse> Certifications { get; init; } = new List<CertificationResponse>();
    }
}