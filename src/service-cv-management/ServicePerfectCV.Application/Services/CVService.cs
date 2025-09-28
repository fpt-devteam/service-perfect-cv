using AutoMapper;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Application.DTOs.Project.Requests;
using System.Runtime.ConstrainedExecution;
using ServicePerfectCV.Application.DTOs.Certification.Requests;

namespace ServicePerfectCV.Application.Services
{
    public class CVService(
        ICVRepository cvRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IEducationRepository educationRepository,
        IExperienceRepository experienceRepository,
        ISkillRepository skillRepository,
        IProjectRepository projectRepository,
        ICertificationRepository certificationRepository,
        IContactRepository contactRepository,
        ISummaryRepository summaryRepository,
        JobDescriptionService jobDescriptionService,
        IMapper mapper
    )
    {
        public async Task<CVResponse> CreateAsync(CreateCVRequest request, Guid userId)
        {
            CV newCV = new CV
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await cvRepository.CreateAsync(newCV);
            await cvRepository.SaveChangesAsync();

            JobDescription createdJobDescription = await jobDescriptionService.CreateAsync(
                jobDescription: new JobDescription
                {
                    Id = Guid.NewGuid(),
                    CVId = newCV.Id,
                    Title = request.JobDescription.Title,
                    CompanyName = request.JobDescription.CompanyName,
                    Responsibility = request.JobDescription.Responsibility,
                    Qualification = request.JobDescription.Qualification
                }
            );

            newCV.JobDescription = createdJobDescription;
            await jobDescriptionService.EnqueueBuildRubricJobAsync(newCV.Id);

            return mapper.Map<CVResponse>(newCV);
        }
        public async Task<PaginationData<CVResponse>> ListAsync(CVQuery query, Guid userId)
        {
            var cvs = await cvRepository.GetByUserIdAsync(query, userId);
            return new PaginationData<CVResponse>
            {
                Total = cvs.Count,
                Items = [.. cvs.Items.Select(cv => mapper.Map<CVResponse>(cv))]
            };
        }
        public async Task<CVResponse> UpdateAsync(Guid cvId, UpdateCVRequest request, Guid userId)
        {
            var cv = await cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            cv.Title = request.Title ?? cv.Title;
            cv.UpdatedAt = DateTime.UtcNow;

            if (request.JobDescription != null)
            {
                var jobDescription = await jobDescriptionRepository.GetByCVIdAsync(cvId) ??
                    throw new DomainException(CVErrors.JobDescriptionNotFound);

                jobDescription.Title = request.JobDescription.Title ?? jobDescription.Title;
                jobDescription.CompanyName = request.JobDescription.CompanyName ?? jobDescription.CompanyName;
                jobDescription.Responsibility = request.JobDescription.Responsibility ?? jobDescription.Responsibility;
                jobDescription.Qualification = request.JobDescription.Qualification ?? jobDescription.Qualification;

                cv.JobDescription = jobDescription;
            }

            cvRepository.Update(cv);
            await cvRepository.SaveChangesAsync();

            return mapper.Map<CVResponse>(cv);
        }
        public async Task<CVResponse> GetByIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            var cv = await cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            var jobDescription = await jobDescriptionRepository.GetByCVIdAsync(cvId) ??
                throw new DomainException(CVErrors.JobDescriptionNotFound);
            cv.JobDescription = jobDescription;
            cv.Content = await GenerateCVContentAsync(cvId, userId);

            return mapper.Map<CVResponse>(cv);
        }
        public async Task DeleteAsync(Guid cvId, Guid userId)
        {
            var deleted = await cvRepository.DeleteByCVIdAndUserIdAsync(cvId, userId);
            if (!deleted)
                throw new DomainException(CVErrors.CVNotFound);

            await jobDescriptionRepository.DeleteByCVIdAsync(cvId);
        }
        private async Task<CVContent> GenerateCVContentAsync(Guid cvId, Guid userId)
        {
            var contact = await contactRepository.GetByCVIdAsync(cvId);
            var summary = await summaryRepository.GetByCVIdAsync(cvId);
            var educations = await educationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new EducationQuery());
            var experiences = await experienceRepository.GetByCVIdAndUserIdAsync(cvId, userId);
            var skills = await skillRepository.GetByCVIdAndUserIdAsync(cvId, userId, new SkillQuery());
            var projects = await projectRepository.GetByCVIdAndUserIdAsync(cvId, userId, new ProjectQuery());
            var certifications = await certificationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new CertificationQuery());

            return new CVContent
            {
                Contact = contact?.ToContactInfo(),
                Summary = summary?.ToSummaryInfo(),
                Educations = educations?.Select(e => e.ToEducationInfo()).ToList() ?? new List<EducationInfo>(),
                Experiences = experiences?.Select(e => e.ToExperienceInfo()).ToList() ?? new List<ExperienceInfo>(),
                Skills = skills?.Select(s => s.ToSkillInfo()).ToList() ?? new List<SkillInfo>(),
                Projects = projects?.Select(p => p.ToProjectInfo()).ToList() ?? new List<ProjectInfo>(),
                Certifications = certifications?.Select(c => c.ToCertificationInfo()).ToList() ?? new List<CertificationInfo>()
            };
        }

    }
}