using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Request;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class ItemRepository : CrudRepositoryBase<Item, Guid>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Item?> GetByIdAsync(Guid id)
        {
            var item = await _context.Items.Where(x => x.DeletedAt == null)
                .FirstOrDefaultAsync(i => i.Id == id);

            return item;

        }

        public async Task<PaginationData<Item>> ListAllAsync(PaginationRequest request)
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

        public async Task<Guid?> GetFirstInsufficientStockItemIdAsync(
            List<OrderItemRequest> items)
        {
            var ids = items.Select(x => x.ItemId).ToList();
            var dbItems = await _context.Items
                                        .Where(i => ids.Contains(i.Id) && i.DeletedAt == null)
                                        .ToListAsync();

            foreach (var req in items)
            {
                var db = dbItems.FirstOrDefault(i => i.Id == req.ItemId);
                if (db != null && db.Quantity < req.Quantity)
                    return req.ItemId;
            }
            return null;
        }

        public async Task<List<Item>> GetByIdsAsync(List<Guid> ids)
        {
            var items = await _context.Items
                .Where(x => ids.Contains(x.Id) && x.DeletedAt == null)
                .ToListAsync();

            return items;
        }
    }

}