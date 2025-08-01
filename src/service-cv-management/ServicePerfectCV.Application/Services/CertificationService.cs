using AutoMapper;
using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class CertificationService
    {
        private readonly ICertificationRepository _certificationRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ICVSnapshotService _cvSnapshotService;
        private readonly NotificationService _notificationService;

        public CertificationService(
            ICertificationRepository certificationRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IOrganizationRepository organizationRepository,
            ICVSnapshotService cvSnapshotService,
            NotificationService notificationService)
        {
            _certificationRepository = certificationRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _organizationRepository = organizationRepository;
            _cvSnapshotService = cvSnapshotService;
            _notificationService = notificationService;
        }

        public async Task<CertificationResponse> CreateAsync(Guid cvId, CreateCertificationRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(id: cvId);
            if (cv == null)
                throw new DomainException(CertificationErrors.CVNotFound);

            var newCertification = _mapper.Map<Certification>(source: request);
            newCertification.Id = Guid.NewGuid();
            newCertification.CVId = cvId;
            newCertification.OrganizationId = (await _organizationRepository.GetByNameAsync(request.Organization))?.Id;

            await _certificationRepository.CreateAsync(entity: newCertification);
            await _certificationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(cvId);

            // Send notification
            _notificationService.SendCertificationUpdateNotificationAsync(cv.UserId, "added");

            return _mapper.Map<CertificationResponse>(source: newCertification);
        }

        public async Task<CertificationResponse> UpdateAsync(Guid certificationId, UpdateCertificationRequest request)
        {
            var existingCertification = await _certificationRepository.GetByIdAsync(id: certificationId);
            if (existingCertification == null)
                throw new DomainException(CertificationErrors.NotFound);

            var cv = await _cvRepository.GetByIdAsync(existingCertification.CVId);
            if (cv == null)
                throw new DomainException(CertificationErrors.CVNotFound);

            existingCertification.Name = request.Name;
            existingCertification.OrganizationId = (await _organizationRepository.GetByNameAsync(request.Organization))?.Id;
            existingCertification.Organization = request.Organization;
            existingCertification.IssuedDate = request.IssuedDate;
            existingCertification.Description = request.Description;
            existingCertification.UpdatedAt = DateTime.UtcNow;

            _certificationRepository.Update(entity: existingCertification);
            await _certificationRepository.SaveChangesAsync();

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(existingCertification.CVId);

            // Send notification
            await _notificationService.SendCertificationUpdateNotificationAsync(cv.UserId, "updated");

            return _mapper.Map<CertificationResponse>(source: existingCertification);
        }

        public async Task<CertificationResponse> GetByIdAsync(Guid certificationId, Guid cvId, Guid userId)
        {
            var certification = await _certificationRepository.GetByIdAndCVIdAndUserIdAsync(
                certificationId: certificationId,
                cvId: cvId,
                userId: userId);

            if (certification == null)
                throw new DomainException(CertificationErrors.NotFound);

            return _mapper.Map<CertificationResponse>(source: certification);
        }

        public async Task<IEnumerable<CertificationResponse>> GetByCVIdAsync(Guid cvId, Guid userId, CertificationQuery query)
        {
            var cv = await _cvRepository.GetByIdAsync(id: cvId);
            if (cv == null || cv.UserId != userId)
                throw new DomainException(CertificationErrors.CVNotFound);

            var certifications = await _certificationRepository.GetByCVIdAndUserIdAsync(
                cvId: cvId,
                userId: userId,
                query: query);

            return _mapper.Map<IEnumerable<CertificationResponse>>(source: certifications);
        }

        public async Task DeleteAsync(Guid certificationId, Guid cvId, Guid userId)
        {
            var certification = await _certificationRepository.GetByIdAndCVIdAndUserIdAsync(
                certificationId: certificationId,
                cvId: cvId,
                userId: userId);

            if (certification == null)
                throw new DomainException(CertificationErrors.NotFound);

            certification.DeletedAt = DateTime.UtcNow;
            _certificationRepository.Update(entity: certification);
            await _certificationRepository.SaveChangesAsync();
            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(certification.CVId);

            // Send notification
            await _notificationService.SendCertificationUpdateNotificationAsync(userId, "deleted");
        }
    }
}
