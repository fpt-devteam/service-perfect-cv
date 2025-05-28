using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
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
    public class OrderRepository(ApplicationDbContext context) : CrudRepositoryBase<Order, Guid>(context), IOrderRepository
    {
        public async Task<Order?> GetByIdAsync(Guid id)
        {

            return await _context.Orders.Where(x => x.Id == id)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync();
        }

        public async Task<PaginationData<Order>> ListAsync(PaginationRequest request)
        {
            var query = await _context.Orders.Include(x => x.OrderItems)
                                                .ThenInclude(x => x.Item).ToListAsync();
            var totalCount = query.Count;
            var items = query.Skip(request.Offset)
                .Take(request.Limit)
                .ToList();
            return new PaginationData<Order>
            {
                Total = totalCount,
                Items = items
            };

        }


    }
}