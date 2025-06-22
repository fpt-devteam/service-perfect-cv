using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICVRepository : IGenericRepository<CV, Guid>
    {
        Task<PaginationData<CV>> ListAsync(PaginationQuery paginationQuery, Guid userId);
    }
}