using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Pagination.Request;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order, Guid>
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<PaginationData<Order>> ListAllAsync(PaginationRequest request);
    }
}
