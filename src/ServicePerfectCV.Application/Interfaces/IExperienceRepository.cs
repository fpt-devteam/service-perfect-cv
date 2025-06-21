using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IExperienceRepository : IGenericRepository<Experience, Guid>
    {
        Task<IEnumerable<Experience>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId);
        Task<Experience?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId);
    }
}
