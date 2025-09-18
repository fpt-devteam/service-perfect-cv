using AutoMapper;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class ExperienceService
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IEmploymentTypeRepository _employmentTypeRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly NotificationService _notificationService;

        public ExperienceService(
            IExperienceRepository experienceRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IJobTitleRepository jobTitleRepository,
            IEmploymentTypeRepository employmentTypeRepository,
            IOrganizationRepository organizationRepository,
            NotificationService notificationService)
        {
            _experienceRepository = experienceRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _jobTitleRepository = jobTitleRepository;
            _employmentTypeRepository = employmentTypeRepository;
            _organizationRepository = organizationRepository;
            _notificationService = notificationService;
        }

        public async Task<ExperienceResponse> CreateAsync(Guid cvId, CreateExperienceRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(cvId);
            if (cv == null)
                throw new DomainException(ExperienceErrors.CVNotFound);

            var employmentType = await _employmentTypeRepository.GetByIdAsync(request.EmploymentTypeId);
            if (employmentType == null)
                throw new DomainException(EmploymentTypeErrors.NotFound);

            var newExperience = _mapper.Map<Experience>(request);
            newExperience.CVId = cvId;

            await _experienceRepository.CreateAsync(newExperience);
            await _experienceRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendExperienceUpdateNotificationAsync(cv.UserId, "added");

            return _mapper.Map<ExperienceResponse>(newExperience);
        }

        public async Task<ExperienceResponse> UpdateAsync(Guid experienceId, UpdateExperienceRequest request)
        {
            var existingExperience = await _experienceRepository.GetByIdAsync(experienceId);
            if (existingExperience == null)
                throw new DomainException(ExperienceErrors.NotFound);

            var cv = await _cvRepository.GetByIdAsync(existingExperience.CVId);
            if (cv == null)
                throw new DomainException(ExperienceErrors.CVNotFound);

            var employmentType = await _employmentTypeRepository.GetByIdAsync(request.EmploymentTypeId);
            if (employmentType == null)
                throw new DomainException(EmploymentTypeErrors.NotFound);

            existingExperience.JobTitle = request.JobTitle;
            existingExperience.EmploymentTypeId = employmentType.Id;
            existingExperience.Organization = request.Organization;
            existingExperience.Location = request.Location;
            existingExperience.StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
            existingExperience.EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue);
            existingExperience.Description = request.Description;
            existingExperience.UpdatedAt = DateTime.UtcNow;

            _experienceRepository.Update(existingExperience);
            await _experienceRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendExperienceUpdateNotificationAsync(cv.UserId, "updated");

            return _mapper.Map<ExperienceResponse>(existingExperience);
        }

        public async Task<ExperienceResponse> GetByIdAsync(Guid experienceId, Guid cvId, Guid userId)
        {
            var experience = await _experienceRepository.GetByIdAndCVIdAndUserIdAsync(experienceId, cvId, userId);
            if (experience == null)
                throw new DomainException(ExperienceErrors.NotFound);

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<IEnumerable<ExperienceResponse>> GetByCVIdAsync(Guid cvId, Guid userId)
        {
            var cv = await _cvRepository.GetByIdAsync(cvId);
            if (cv == null || cv.UserId != userId)
                throw new DomainException(ExperienceErrors.NotFound);

            var experiences = await _experienceRepository.GetByCVIdAndUserIdAsync(cvId, userId);
            return _mapper.Map<IEnumerable<ExperienceResponse>>(experiences);
        }

        public async Task DeleteAsync(Guid experienceId, Guid cvId, Guid userId)
        {
            var experience = await _experienceRepository.GetByIdAndCVIdAndUserIdAsync(experienceId, cvId, userId);
            if (experience == null)
                throw new DomainException(ExperienceErrors.NotFound);

            experience.DeletedAt = DateTime.UtcNow;
            _experienceRepository.Update(experience);
            await _experienceRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendExperienceUpdateNotificationAsync(userId, "deleted");
        }
    }
}