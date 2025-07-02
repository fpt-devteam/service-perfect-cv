using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.Common;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICVRepository : IGenericRepository<CV, Guid>
    {
        Task<FilterView<CV>> GetByUserIdAsync(CVQuery cvQuery, Guid userId);
        Task<CV?> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId);
    }
}