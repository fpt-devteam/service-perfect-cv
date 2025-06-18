using AutoMapper;
using ServicePerfectCV.Application.Common;
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
        private readonly ICompanyRepository _companyRepository;

        public ExperienceService(
            IExperienceRepository experienceRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IJobTitleRepository jobTitleRepository,
            IEmploymentTypeRepository employmentTypeRepository,
            ICompanyRepository companyRepository)
        {
            _experienceRepository = experienceRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _jobTitleRepository = jobTitleRepository;
            _employmentTypeRepository = employmentTypeRepository;
            _companyRepository = companyRepository;
        }

        public async Task<ExperienceResponse> CreateAsync(CreateExperienceRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(request.CVId);
            if (cv == null)
                throw new DomainException(ExperienceErrors.CVNotFound);

            if (request.JobTitleId.HasValue)
            {
                if (await _jobTitleRepository.GetByIdAsync(request.JobTitleId.Value) == null)
                    throw new DomainException(JobTitleErrors.NotFound);
            }

            if (request.CompanyId.HasValue)
            {
                if (await _companyRepository.GetByIdAsync(request.CompanyId.Value) == null)
                    throw new DomainException(CompanyErrors.NotFound);
            }

            var employmentType = await _employmentTypeRepository.GetByIdAsync(request.EmploymentTypeId);
            if (employmentType == null)
                throw new DomainException(EmploymentTypeErrors.NotFound);

            var newExperience = _mapper.Map<Experience>(request);

            await _experienceRepository.CreateAsync(newExperience);
            await _experienceRepository.SaveChangesAsync();

            return _mapper.Map<ExperienceResponse>(newExperience);
        }

        public async Task<ExperienceResponse> UpdateAsync(Guid experienceId, UpdateExperienceRequest request)
        {
            var existingExperience = await _experienceRepository.GetByIdAsync(experienceId);
            if (existingExperience == null)
                throw new DomainException(ExperienceErrors.NotFound);

            if (request.JobTitleId.HasValue)
            {
                var jobTitle = await _jobTitleRepository.GetByIdAsync(request.JobTitleId.Value);
                if (jobTitle == null)
                    throw new DomainException(JobTitleErrors.NotFound);
            }

            var employmentType = await _employmentTypeRepository.GetByIdAsync(request.EmploymentTypeId);
            if (employmentType == null)
                throw new DomainException(EmploymentTypeErrors.NotFound);

            if (request.CompanyId.HasValue)
            {
                var company = await _companyRepository.GetByIdAsync(request.CompanyId.Value);
                if (company == null)
                    throw new DomainException(CompanyErrors.NotFound);
            }

            existingExperience.JobTitle = request.JobTitle;
            existingExperience.JobTitleId = request.JobTitleId;
            existingExperience.EmploymentTypeId = request.EmploymentTypeId;
            existingExperience.Company = request.Company;
            existingExperience.CompanyId = request.CompanyId;
            existingExperience.Location = request.Location;
            existingExperience.StartDate = request.StartDate;
            existingExperience.EndDate = request.EndDate;
            existingExperience.Description = request.Description;
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

            var experiences = await _experienceRepository.ListByCVIdAndUserIdAsync(cvId, userId);
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