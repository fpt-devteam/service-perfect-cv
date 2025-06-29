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

        public CertificationService(
            ICertificationRepository certificationRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            IOrganizationRepository organizationRepository,
            ICVSnapshotService cvSnapshotService)
        {
            _certificationRepository = certificationRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _organizationRepository = organizationRepository;
            _cvSnapshotService = cvSnapshotService;
        }

        public async Task<CertificationResponse> CreateAsync(CreateCertificationRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(id: request.CVId);
            if (cv == null)
                throw new DomainException(CertificationErrors.CVNotFound);

            if (request.OrganizationId.HasValue && await _organizationRepository.GetByIdAsync(id: request.OrganizationId.Value) == null)
                throw new DomainException(CertificationErrors.OrganizationNotFound);

            var newCertification = _mapper.Map<Certification>(source: request);
            newCertification.Id = Guid.NewGuid();

            await _certificationRepository.CreateAsync(entity: newCertification);
            await _certificationRepository.SaveChangesAsync();

            // Update CV snapshot after creating certification
            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(request.CVId);

            return _mapper.Map<CertificationResponse>(source: newCertification);
        }

        public async Task<CertificationResponse> UpdateAsync(Guid certificationId, UpdateCertificationRequest request)
        {
            var existingCertification = await _certificationRepository.GetByIdAsync(id: certificationId);
            if (existingCertification == null)
                throw new DomainException(CertificationErrors.NotFound);

            if (request.OrganizationId.HasValue && await _organizationRepository.GetByIdAsync(id: request.OrganizationId.Value) == null)
                throw new DomainException(CertificationErrors.OrganizationNotFound);

            existingCertification.Name = request.Name;
            existingCertification.Organization = request.Organization;
            existingCertification.OrganizationId = request.OrganizationId;
            existingCertification.IssuedDate = request.IssuedDate;
            existingCertification.Description = request.Description;
            existingCertification.UpdatedAt = DateTime.UtcNow;

            _certificationRepository.Update(entity: existingCertification);
            await _certificationRepository.SaveChangesAsync();

            // Update CV snapshot after updating certification
            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(existingCertification.CVId);

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

            // Update CV snapshot after deleting certification
            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(certification.CVId);
        }
    }
}
