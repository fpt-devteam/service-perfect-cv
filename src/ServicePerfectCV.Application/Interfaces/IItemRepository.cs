using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IItemRepository : IGenericRepository<Item, Guid>
    {
        Task<Item?> GetByIdAsync(Guid id);
        Task<PaginationData<Item>> ListAsync(PaginationRequest request);
        Task<IEnumerable<Item>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<Guid?> UpdateQuantityStock(IEnumerable<OrderItemRequest> itemRequests);
    }
}