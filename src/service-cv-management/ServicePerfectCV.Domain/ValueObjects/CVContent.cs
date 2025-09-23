using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public required string Content { get; set; }
    }

    public class EducationInfo
    {
        public required string Degree { get; set; }
        public required string Organization { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? Gpa { get; set; }
    }

    public class ExperienceInfo
    {
        public required string JobTitle { get; set; }
        public required Guid EmploymentTypeId { get; set; }
        public required string Organization { get; set; }
        public string? Location { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectInfo
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Link { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class SkillInfo
    {
        public required string Category { get; set; }
        public required string Content { get; set; }
    }

    public class CertificationInfo
    {
        public required string Name { get; set; }
        public required string Organization { get; set; }
        public DateOnly? IssuedDate { get; set; }
        public string? Description { get; set; }
    }

    public static class CVContentExtensions
    {
        public static EducationInfo ToEducationInfo(this Education education)
        {
            return new EducationInfo
            {
                Degree = education.Degree,
                Organization = education.Organization,
                FieldOfStudy = education.FieldOfStudy,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Description = education.Description,
                Gpa = education.Gpa
            };
        }

        public static ExperienceInfo ToExperienceInfo(this Experience experience)
        {
            return new ExperienceInfo
            {
                JobTitle = experience.JobTitle,
                EmploymentTypeId = experience.EmploymentTypeId,
                Organization = experience.Organization,
                Location = experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description
            };
        }

        public static ProjectInfo ToProjectInfo(this Project project)
        {
            return new ProjectInfo
            {
                Title = project.Title,
                Description = project.Description,
                Link = project.Link,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };
        }

        public static SkillInfo ToSkillInfo(this Skill skill)
        {
            return new SkillInfo
            {
                Category = skill.Category,
                Content = skill.Content
            };
        }

        public static CertificationInfo ToCertificationInfo(this Certification certification)
        {
            return new CertificationInfo
            {
                Name = certification.Name,
                Organization = certification.Organization,
                IssuedDate = certification.IssuedDate,
                Description = certification.Description
            };
        }

        public static ContactInfo ToContactInfo(this Contact contact)
        {
            return new ContactInfo
            {
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                LinkedInUrl = contact.LinkedInUrl,
                GitHubUrl = contact.GitHubUrl,
                PersonalWebsiteUrl = contact.PersonalWebsiteUrl,
                Country = contact.Country,
                City = contact.City
            };
        }

        public static SummaryInfo ToSummaryInfo(this Summary summary)
        {
            return new SummaryInfo
            {
                Content = summary.Content
            };
        }
    }
}