using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class ItemRepository(ApplicationDbContext context) : CrudRepositoryBase<Item, Guid>(context), IItemRepository
    {
        public async Task<Item?> GetByIdAsync(Guid id)
        {
            var item = await _context.Items.Where(x => x.DeletedAt == null)
                .FirstOrDefaultAsync(i => i.Id == id);

            return item;

        }

        public async Task<PaginationData<Item>> ListAsync(PaginationRequest request)
        {
            var query = await _context.Items.Where(x => x.DeletedAt == null).ToListAsync();
            var totalCount = query.Count;
            var items = query.Skip(request.Offset)
                .Take(request.Limit)
                .ToList();
            return new PaginationData<Item>
            {
                Total = totalCount,
                Items = items
            };

        }

        public async Task<Guid?> UpdateQuantityStock(IEnumerable<OrderItemRequest> itemRequests)
        {
            var itemIds = itemRequests.Select(r => r.ItemId).ToList();
            var dbItems = await _context.Items
                                      .Where(i => itemIds.Contains(i.Id) && i.DeletedAt == null)
                                      .ToDictionaryAsync(i => i.Id);

            foreach (var req in itemRequests)
            {
                if (!dbItems.TryGetValue(req.ItemId, out var dbItem) || dbItem.Quantity < req.Quantity)
                    return req.ItemId;

                dbItem.Quantity -= req.Quantity;
                _context.Items.Update(dbItem);
            }

            return null;
        }

        public async Task<IEnumerable<Item>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var items = await _context.Items
                .Where(x => ids.Contains(x.Id) && x.DeletedAt == null)
                .ToListAsync();
            return items;
        }

    }

}