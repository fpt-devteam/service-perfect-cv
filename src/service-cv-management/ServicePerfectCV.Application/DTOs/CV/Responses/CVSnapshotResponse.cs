using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class CVSnapshotResponse
    {
        public Guid UserId { get; init; }
        public string Title { get; init; } = default!;
        public JobDetailDto? JobDetail { get; init; }
        public ContactResponse? Contacts { get; init; }
        //TODO: SummaryResponse and SkillsResponse
        public IEnumerable<EducationResponse> Educations { get; init; } = null!;
        public IEnumerable<CertificationResponse> Certifications { get; init; } = null!;
        public IEnumerable<ExperienceResponse> Experiences { get; init; } = null!;
        public IEnumerable<ProjectResponse> Projects { get; init; } = null!;



    }
}