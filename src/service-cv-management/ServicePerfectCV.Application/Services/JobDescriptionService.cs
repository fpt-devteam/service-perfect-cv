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
using ServicePerfectCV.Application.DTOs.CV.Requests;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Interfaces.Repositories;

namespace ServicePerfectCV.Application.Services
{
    public class JobDescriptionService
    {
        private readonly IJobDescriptionRepository _jobDescriptionRepository;
        private readonly JobService _jobService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<JobDescriptionService> _logger;

        public JobDescriptionService(IJobDescriptionRepository jobDescriptionRepository, JobService jobService, IJsonHelper jsonHelper, ILogger<JobDescriptionService> logger)
        {
            _jobDescriptionRepository = jobDescriptionRepository;
            _jobService = jobService;
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public async Task<JobDescription> CreateAsync(JobDescription jobDescription)
        {
            var existingJobDescription = await _jobDescriptionRepository.GetByCVIdAsync(jobDescription.CVId);
            if (existingJobDescription != null)
                throw new DomainException(CVErrors.JobDescriptionAlreadyExists with { Message = $"A job description already exists for CV with ID {jobDescription.CVId}." });

            var createdJobDescription = await _jobDescriptionRepository.CreateAsync(jobDescription);

            await _jobDescriptionRepository.SaveChangesAsync();

            await EnqueueBuildRubricJobAsync(createdJobDescription.Id);

            return createdJobDescription;
        }

        public async Task<JobDescription> UpdateAsync(Guid id, UpdateJobDescriptionRequest request)
        {
            var jobDescription = await _jobDescriptionRepository.GetByIdAsync(id);
            if (jobDescription == null)
                throw new DomainException(CVErrors.JobDescriptionNotFound with { Message = $"Job description with ID {id} not found." });

            jobDescription.Title = request.Title ?? jobDescription.Title;
            jobDescription.CompanyName = request.CompanyName ?? jobDescription.CompanyName;
            jobDescription.Responsibility = request.Responsibility ?? jobDescription.Responsibility;
            jobDescription.Qualification = request.Qualification ?? jobDescription.Qualification;

            var updateResult = _jobDescriptionRepository.Update(jobDescription);
            if (!updateResult)
                throw new DomainException(CVErrors.JobDescriptionUpdateFailed with { Message = "Failed to update job description." });

            await _jobDescriptionRepository.SaveChangesAsync();

            await EnqueueBuildRubricJobAsync(jobDescription.Id);

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

            _logger.LogInformation("Enqueued BuildCvSectionRubric job with ID {JobId} for JobDescription ID {JobDescriptionId}", job.Id, jobDescriptionId);

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
