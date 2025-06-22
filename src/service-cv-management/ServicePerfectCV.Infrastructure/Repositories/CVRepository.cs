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
    public class CVRepository(ApplicationDbContext context) : CrudRepositoryBase<CV, Guid>(context), ICVRepository
    {
        public async Task<PaginationData<CV>> ListAsync(PaginationQuery paginationQuery, Guid userId)
        {
            var query = _context.CVs.Where(cv => cv.UserId == userId).Include(cv => cv.Educations);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(paginationQuery.Offset)
                .Take(paginationQuery.Limit)
                .ToListAsync();
            return new PaginationData<CV>
            {
                Items = items,
                Total = totalCount
            };
        }
    }
}