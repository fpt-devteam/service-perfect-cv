using AutoMapper;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class EducationService
    {
        private readonly IEducationRepository _educationRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;
        private readonly IDegreeRepository _degreeRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ICVSnapshotService _cvSnapshotService;

        public EducationService(
            IEducationRepository educationRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IDegreeRepository degreeRepository,
            IOrganizationRepository organizationRepository,
            ICVSnapshotService cvSnapshotService)
        {
            _educationRepository = educationRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _degreeRepository = degreeRepository;
            _organizationRepository = organizationRepository;
            _cvSnapshotService = cvSnapshotService;
        }

        public async Task<EducationResponse> CreateAsync(CreateEducationRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(request.CVId);
            if (cv == null)
                throw new DomainException(EducationErrors.CVNotFound);

            if (request.DegreeId.HasValue && await _degreeRepository.GetByIdAsync(request.DegreeId.Value) == null)
                throw new DomainException(EducationErrors.DegreeNotFound);

            if (request.OrganizationId.HasValue && await _organizationRepository.GetByIdAsync(request.OrganizationId.Value) == null)
                throw new DomainException(EducationErrors.OrganizationNotFound);

            var newEducation = _mapper.Map<Education>(request);

            await _educationRepository.CreateAsync(newEducation);
            await _educationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(request.CVId);

            return _mapper.Map<EducationResponse>(newEducation);
        }

        public async Task<EducationResponse> UpdateAsync(Guid educationId, UpdateEducationRequest request)
        {
            var existingEducation = await _educationRepository.GetByIdAsync(educationId);
            if (existingEducation == null)
                throw new DomainException(EducationErrors.NotFound);

            if (request.DegreeId.HasValue && await _degreeRepository.GetByIdAsync(request.DegreeId.Value) == null)
                throw new DomainException(EducationErrors.DegreeNotFound);

            if (request.OrganizationId.HasValue && await _organizationRepository.GetByIdAsync(request.OrganizationId.Value) == null)
                throw new DomainException(EducationErrors.OrganizationNotFound);

            existingEducation.Degree = request.Degree;
            existingEducation.DegreeId = request.DegreeId;
            existingEducation.Organization = request.Organization;
            existingEducation.OrganizationId = request.OrganizationId;
            existingEducation.FieldOfStudy = request.FieldOfStudy;
            existingEducation.StartDate = request.StartDate;
            existingEducation.EndDate = request.EndDate;
            existingEducation.Description = request.Description;
            existingEducation.Gpa = request.Gpa;
            existingEducation.UpdatedAt = DateTime.UtcNow;

            _educationRepository.Update(existingEducation);
            await _educationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(existingEducation.CVId);

            return _mapper.Map<EducationResponse>(existingEducation);
        }

        public async Task<EducationResponse> GetByIdAsync(Guid educationId, Guid cvId, Guid userId)
        {
            var education = await _educationRepository.GetByIdAndCVIdAndUserIdAsync(educationId, cvId, userId);
            if (education == null)
                throw new DomainException(EducationErrors.NotFound);

            return _mapper.Map<EducationResponse>(education);
        }

        public async Task<IEnumerable<EducationResponse>> GetByCVIdAsync(Guid cvId, Guid userId, EducationQuery query)
        {
            var cv = await _cvRepository.GetByIdAsync(id: cvId);
            if (cv == null || cv.UserId != userId)
                throw new DomainException(EducationErrors.CVNotFound);

            var educations = await _educationRepository.GetByCVIdAndUserIdAsync(cvId: cvId, userId: userId, query: query);
            return _mapper.Map<IEnumerable<EducationResponse>>(educations);
        }

        public async Task DeleteAsync(Guid educationId, Guid cvId, Guid userId)
        {
            var education = await _educationRepository.GetByIdAndCVIdAndUserIdAsync(educationId, cvId, userId);
            if (education == null)
                throw new DomainException(EducationErrors.NotFound);

            education.DeletedAt = DateTime.UtcNow;
            _educationRepository.Update(education);
            await _educationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(education.CVId);
        }
    }
}