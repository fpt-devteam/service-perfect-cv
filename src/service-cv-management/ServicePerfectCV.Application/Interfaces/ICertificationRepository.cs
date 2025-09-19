using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICertificationRepository : IGenericRepository<Certification, Guid>
    {
        Task<Certification?> GetByIdAndCVIdAndUserIdAsync(Guid certificationId, Guid cvId, Guid userId);
        Task<IEnumerable<Certification>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, CertificationQuery query);
    }
}