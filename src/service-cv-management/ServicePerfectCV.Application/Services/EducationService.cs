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
        private readonly NotificationService _notificationService;

        public EducationService(
            IEducationRepository educationRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IDegreeRepository degreeRepository,
            IOrganizationRepository organizationRepository,
            ICVSnapshotService cvSnapshotService,
            NotificationService notificationService)
        {
            _educationRepository = educationRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _degreeRepository = degreeRepository;
            _organizationRepository = organizationRepository;
            _cvSnapshotService = cvSnapshotService;
            _notificationService = notificationService;
        }

        public async Task<EducationResponse> CreateAsync(Guid cvId, CreateEducationRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(cvId);
            if (cv == null)
                throw new DomainException(EducationErrors.CVNotFound);

            var newEducation = _mapper.Map<Education>(request);
            newEducation.CVId = cvId;
            newEducation.DegreeId = (await _degreeRepository.GetByNameAsync(request.Degree))?.Id;
            newEducation.OrganizationId = (await _organizationRepository.GetByNameAsync(request.Organization))?.Id;

            await _educationRepository.CreateAsync(newEducation);
            await _educationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(cvId);

            // Send notification
            await _notificationService.SendEducationUpdateNotificationAsync(cv.UserId, "added");

            return _mapper.Map<EducationResponse>(newEducation);
        }

        public async Task<EducationResponse> UpdateAsync(Guid educationId, UpdateEducationRequest request)
        {
            var existingEducation = await _educationRepository.GetByIdAsync(educationId);
            if (existingEducation == null)
                throw new DomainException(EducationErrors.NotFound);

            var cv = await _cvRepository.GetByIdAsync(existingEducation.CVId);
            if (cv == null)
                throw new DomainException(EducationErrors.CVNotFound);

            var degree = await _degreeRepository.GetByNameAsync(request.Degree);
            var organization = await _organizationRepository.GetByNameAsync(request.Organization);

            existingEducation.Degree = request.Degree;
            existingEducation.DegreeId = degree?.Id;
            existingEducation.Organization = request.Organization;
            existingEducation.OrganizationId = organization?.Id;
            existingEducation.FieldOfStudy = request.FieldOfStudy;
            existingEducation.StartDate = request.StartDate;
            existingEducation.EndDate = request.EndDate;
            existingEducation.Description = request.Description;
            existingEducation.Gpa = request.Gpa;
            existingEducation.UpdatedAt = DateTime.UtcNow;

            _educationRepository.Update(existingEducation);
            await _educationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(existingEducation.CVId);

            // Send notification
            await _notificationService.SendEducationUpdateNotificationAsync(cv.UserId, "updated");

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

            // Send notification
            await _notificationService.SendEducationUpdateNotificationAsync(userId, "deleted");
        }
    }
}