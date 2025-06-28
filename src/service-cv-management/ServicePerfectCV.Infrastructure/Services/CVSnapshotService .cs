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
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class CVSnapshotService(
        ApplicationDbContext context,
        ICacheService cacheService
    ) : ICVSnapshotService
    {
        public async Task UpdateCVSnapshotIfChangedAsync(Guid cvId)
        {
            var newDto = await context.CVs
            .Where(cv => cv.Id == cvId)
            .AsNoTracking()
            .Select(cv => new CVSnapshotResponse
            {
                UserId = cv.UserId,
                Title = cv.Title,
                JobDetail = cv.JobDetail != null ? new JobDetailDto
                {
                    JobTitle = cv.JobDetail.JobTitle,
                    CompanyName = cv.JobDetail.CompanyName,
                    Description = cv.JobDetail.Description
                } : null,
                Contacts = new ContactResponse
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
                },
                Educations = cv.Educations.Select(e => new EducationResponse
                {
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
                    EmploymentTypeName = e.EmploymentType.Name,
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
            })
            .FirstOrDefaultAsync(); if (newDto == null) return;

            var newJson = JsonSerializer.Serialize(newDto);

            // Get the CV entity to check if content has changed
            var cv = await context.CVs.FindAsync(cvId);
            if (cv == null) return;

            // Compare with existing content
            var isChanged = cv.FullContent != newJson;

            if (isChanged)
            {
                // Update the FullContent field
                cv.FullContent = newJson;
                cv.UpdatedAt = DateTime.UtcNow;

                // Save to cache and database
                await cacheService.SetAsync($"cv:{cvId}", newJson, TimeSpan.FromHours(1));
                await context.SaveChangesAsync();
            }
        }
    }
}