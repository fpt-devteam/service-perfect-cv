using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface IEducationRepository : IGenericRepository<Education, Guid>
    {
        Task<Education?> GetByIdAndCVIdAndUserIdAsync(Guid educationId, Guid cvId, Guid userId);
        Task<IEnumerable<Education>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, EducationQuery query);
    }
}