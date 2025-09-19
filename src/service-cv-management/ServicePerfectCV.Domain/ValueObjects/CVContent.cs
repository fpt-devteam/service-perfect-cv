using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Domain.ValueObjects
{
    public class CVContent
    {
        public ContactInfo? Contact { get; set; }
        public SummaryInfo? Summary { get; set; }
        public List<EducationInfo> Educations { get; set; } = [];
        public List<ExperienceInfo> Experiences { get; set; } = [];
        public List<ProjectInfo> Projects { get; set; } = [];
        public List<SkillInfo> Skills { get; set; } = [];
        public List<CertificationInfo> Certifications { get; set; } = [];
    }

    public class ContactInfo
    {
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PersonalWebsiteUrl { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
    }

    public class SummaryInfo
    {
        public string Context { get; set; } = string.Empty;
    }

    public class EducationInfo
    {
        public string Degree { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? Gpa { get; set; }
    }

    public class ExperienceInfo
    {
        public string JobTitle { get; set; } = string.Empty;
        public Guid? EmploymentTypeId { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectInfo
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SkillInfo
    {
        public string SkillItems { get; set; } = string.Empty;
    }

    public class CertificationInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public DateTime? IssuedDate { get; set; }
        public string? Description { get; set; }
    }
}