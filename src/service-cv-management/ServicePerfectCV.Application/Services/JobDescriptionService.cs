using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Application.DTOs.JobDescription;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class JobDescriptionService
    {
        private readonly IJobDescriptionRepository _jobDescriptionRepository;
        private readonly JobService _jobService;
        private readonly IJsonHelper _jsonHelper;

        public JobDescriptionService(IJobDescriptionRepository jobDescriptionRepository, JobService jobService, IJsonHelper jsonHelper)
        {
            _jobDescriptionRepository = jobDescriptionRepository;
            _jobService = jobService;
            _jsonHelper = jsonHelper;
        }

        public async Task<JobDescription> CreateAsync(JobDescription jobDescription)
        {
            if (jobDescription == null)
                throw new DomainException(CVErrors.JobDescriptionValidationFailed with { Message = "Job description cannot be null." });

            var existingJobDescription = await _jobDescriptionRepository.GetByCVIdAsync(jobDescription.CVId);
            if (existingJobDescription != null)
                throw new DomainException(CVErrors.JobDescriptionAlreadyExists with { Message = $"A job description already exists for CV with ID {jobDescription.CVId}." });

            if (jobDescription.Id == Guid.Empty)
                jobDescription.Id = Guid.NewGuid();

            var createdJobDescription = await _jobDescriptionRepository.CreateAsync(jobDescription);
            await _jobDescriptionRepository.SaveChangesAsync();

            return createdJobDescription;
        }

        public async Task<JobDescription> UpdateAsync(Guid id, JobDescription jobDescription)
        {
            if (id == Guid.Empty)
                throw new DomainException(CVErrors.InvalidJobDescriptionId with { Message = "Job description ID cannot be empty." });

            if (jobDescription.Id != id)
                throw new DomainException(CVErrors.InvalidJobDescriptionId with { Message = "Job description ID must match the provided ID." });

            var existingJobDescription = await _jobDescriptionRepository.GetByIdAsync(id);
            if (existingJobDescription == null)
                throw new DomainException(CVErrors.JobDescriptionNotFound with { Message = $"Job description with ID {id} not found." });

            var updateResult = _jobDescriptionRepository.Update(jobDescription);
            if (!updateResult)
                throw new DomainException(CVErrors.JobDescriptionUpdateFailed with { Message = "Failed to update job description." });

            await _jobDescriptionRepository.SaveChangesAsync();

            return jobDescription;
        }

        public async Task<Job> EnqueueBuildRubricJobAsync(Guid jobDescriptionId, int priority = 0, CancellationToken cancellationToken = default)
        {
            if (jobDescriptionId == Guid.Empty)
                throw new DomainException(CVErrors.InvalidJobDescriptionId with { Message = "Job description ID cannot be empty." });

            var jobDescription = await _jobDescriptionRepository.GetByIdAsync(jobDescriptionId);
            if (jobDescription == null)
                throw new DomainException(CVErrors.JobDescriptionNotFound with { Message = $"Job description with ID {jobDescriptionId} not found." });

            var job = await _jobService.CreateAsync(
                JobType.BuildCvSectionRubric,
                _jsonHelper.SerializeToDocument(jobDescription.ToRubricInputDto()),
                priority,
                cancellationToken);

            return job;
        }

        public async Task<JobDescription> UpdateSectionRubricAsync(Guid jobDescriptionId, SectionRubricDictionary sectionRubric)
        {
            if (jobDescriptionId == Guid.Empty)
                throw new DomainException(CVErrors.InvalidJobDescriptionId with { Message = "Job description ID cannot be empty." });

            var jobDescription = await _jobDescriptionRepository.GetByIdAsync(jobDescriptionId);
            if (jobDescription == null)
                throw new DomainException(CVErrors.JobDescriptionNotFound with { Message = $"Job description with ID {jobDescriptionId} not found." });

            jobDescription.SectionRubrics = sectionRubric;

            var updateResult = _jobDescriptionRepository.Update(jobDescription);
            if (!updateResult)
                throw new DomainException(CVErrors.JobDescriptionUpdateFailed with { Message = "Failed to update job description section rubric." });

            await _jobDescriptionRepository.SaveChangesAsync();

            return jobDescription;
        }
    }
}
