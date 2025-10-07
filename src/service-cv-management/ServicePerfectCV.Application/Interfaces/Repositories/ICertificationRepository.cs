using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface ICertificationRepository : IGenericRepository<Certification, Guid>
    {
        Task<Certification?> GetByIdAndCVIdAndUserIdAsync(Guid certificationId, Guid cvId, Guid userId);
        Task<IEnumerable<Certification>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, CertificationQuery query);
    }
}