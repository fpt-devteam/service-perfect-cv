using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Request;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IItemRepository : IGenericRepository<Item, Guid>
    {
        Task<Item?> GetByIdAsync(Guid id);
        Task<PaginationData<Item>> ListAllAsync(PaginationRequest request);
        Task<List<Item>> GetByIdsAsync(List<Guid> ids);
        Task<Guid?> GetFirstInsufficientStockItemIdAsync(
            List<OrderItemRequest> items);
    }
}
