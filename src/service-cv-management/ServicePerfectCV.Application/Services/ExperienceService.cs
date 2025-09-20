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
        private readonly IEmploymentTypeRepository _employmentTypeRepository;

        public ExperienceService(
            IExperienceRepository experienceRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IEmploymentTypeRepository employmentTypeRepository)
        {
            _experienceRepository = experienceRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _employmentTypeRepository = employmentTypeRepository;
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

            return _mapper.Map<ExperienceResponse>(newExperience);
        }

        public async Task<ExperienceResponse> UpdateAsync(Guid experienceId, UpdateExperienceRequest request)
        {
            var existingExperience = await _experienceRepository.GetByIdAsync(experienceId);
            if (existingExperience == null)
                throw new DomainException(ExperienceErrors.NotFound);

            _ = await _cvRepository.GetByIdAsync(existingExperience.CVId) ?? throw new DomainException(ExperienceErrors.CVNotFound);

            if (request.EmploymentTypeId != null)
                _ = await _employmentTypeRepository.GetByIdAsync(request.EmploymentTypeId.Value) ?? throw new DomainException(EmploymentTypeErrors.NotFound);

            existingExperience.JobTitle = request.JobTitle ?? existingExperience.JobTitle;
            existingExperience.EmploymentTypeId = request.EmploymentTypeId ?? existingExperience.EmploymentTypeId;
            existingExperience.Organization = request.Organization ?? existingExperience.Organization;
            existingExperience.Location = request.Location ?? existingExperience.Location;
            existingExperience.StartDate = request.StartDate ?? existingExperience.StartDate;
            existingExperience.EndDate = request.EndDate ?? existingExperience.EndDate;
            existingExperience.Description = request.Description ?? existingExperience.Description;
            existingExperience.UpdatedAt = DateTime.UtcNow;

            _experienceRepository.Update(existingExperience);
            await _experienceRepository.SaveChangesAsync();

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
        }
    }
}