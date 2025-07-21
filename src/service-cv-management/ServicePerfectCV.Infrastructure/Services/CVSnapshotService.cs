using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.CV;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.DTOs.Category.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class CVSnapshotService(
        ApplicationDbContext context,
        ICacheService cacheService,
        IPushNotificationService pushNotificationService,
        IDeviceTokenRepository deviceTokenRepository
    ) : ICVSnapshotService
    {
        public async Task UpdateCVSnapshotIfChangedAsync(Guid cvId)
        {
            var cv = await context.CVs
                .Where(c => c.Id == cvId)
                .Include(c => c.Contact)
                .Include(c => c.Summary)
                .Include(c => c.Skills)
                    .ThenInclude(s => s.CategoryNavigation)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                    .ThenInclude(e => e.EmploymentType)
                .Include(c => c.Projects)
                .Include(c => c.Certifications)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (cv == null) return;

            var newDto = new CVSnapshotResponse
            {
                CvId = cv.Id,
                UserId = cv.UserId,
                Title = cv.Title,
                VersionId = cv.VersionId,
                AnalysisId = cv.AnalysisId,
                JobDetail = cv.JobDetail != null ? new JobDetailDto
                {
                    JobTitle = cv.JobDetail.JobTitle,
                    CompanyName = cv.JobDetail.CompanyName,
                    Description = cv.JobDetail.Description
                } : null,
                Contacts = cv.Contact != null ? new ContactResponse
                {
                    Id = cv.Contact.Id,
                    CVId = cv.Contact.CVId,
                    PhoneNumber = cv.Contact.PhoneNumber,
                    Email = cv.Contact.Email,
                    LinkedInUrl = cv.Contact.LinkedInUrl,
                    GitHubUrl = cv.Contact.GitHubUrl,
                    PersonalWebsiteUrl = cv.Contact.PersonalWebsiteUrl,
                    Country = cv.Contact.Country,
                    City = cv.Contact.City
                } : null,
                Summary = cv.Summary != null ? new SummaryResponse
                {
                    Id = cv.Summary.Id,
                    CVId = cv.Summary.CVId,
                    Context = cv.Summary.Context
                } : null,
                Skills = cv.Skills.Select(s => new SkillResponse
                {
                    Id = s.Id,
                    Category = s.Category,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }),
                Educations = cv.Educations.Select(e => new EducationResponse
                {
                    Id = e.Id,
                    Organization = e.Organization,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description,
                    Gpa = e.Gpa
                }),
                Experiences = cv.Experiences.Select(e => new ExperienceResponse
                {
                    Id = e.Id,
                    CVId = e.CVId,
                    JobTitleId = e.JobTitleId,
                    JobTitle = e.JobTitle,
                    EmploymentTypeId = e.EmploymentTypeId,
                    EmploymentTypeName = e.EmploymentType?.Name,
                    OrganizationId = e.OrganizationId,
                    Organization = e.Organization,
                    Location = e.Location,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                }),
                Projects = cv.Projects.Select(p => new ProjectResponse
                {
                    Id = p.Id,
                    CVId = p.CVId,
                    Title = p.Title,
                    Description = p.Description,
                    Link = p.Link,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }),
                Certifications = cv.Certifications.Select(c => new CertificationResponse
                {
                    Id = c.Id,
                    CVId = c.CVId,
                    Name = c.Name,
                    OrganizationId = c.OrganizationId,
                    Organization = c.Organization,
                    IssuedDate = c.IssuedDate,
                    Description = c.Description
                })
            };

            var newJson = JsonSerializer.Serialize(newDto);

            var isChanged = cv.FullContent != newJson;

            if (isChanged)
            {
                await context.CVs
                    .Where(c => c.Id == cvId)
                    .ExecuteUpdateAsync(c => c
                        .SetProperty(cv => cv.FullContent, newJson)
                        .SetProperty(cv => cv.VersionId, Guid.NewGuid())
                        .SetProperty(cv => cv.UpdatedAt, DateTime.UtcNow));

                await cacheService.SetAsync($"cv:{cvId}", newJson, TimeSpan.FromHours(1));

                var tokens = await deviceTokenRepository.GetTokensByUserIdAsync(cv.UserId);
                await pushNotificationService.SendAsync(tokens, "CV Updated", "Your CV has been updated.");
            }
        }
    }
}