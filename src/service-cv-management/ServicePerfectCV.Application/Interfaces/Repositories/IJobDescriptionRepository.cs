using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface IJobDescriptionRepository : IGenericRepository<JobDescription, Guid>
    {
        Task<JobDescription?> GetByCVIdAsync(Guid cvId);
        Task<bool> DeleteByCVIdAsync(Guid cvId);
    }
}