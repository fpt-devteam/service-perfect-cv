using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.Common;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class CVRepository(ApplicationDbContext context) : CrudRepositoryBase<CV, Guid>(context), ICVRepository
    {
        public async Task<FilterView<CV>> GetByUserIdAsync(CVQuery query, Guid userId)
        {
            var baseQuery = _context.CVs
                .AsNoTracking()
                .Where(cv => cv.UserId == userId);
            var sorted = query.Sort != null ? ApplySort(baseQuery, query.Sort) : baseQuery;
            var totalCount = await sorted.CountAsync();
            var paged = sorted.Skip(query.Offset).Take(query.Limit);

            return new FilterView<CV>()
            {
                Count = totalCount,
                Items = await paged.ToListAsync()
            };
        }
        private static IQueryable<CV> ApplySort(IQueryable<CV> query, CVSort sort)
        {
            if (sort.UpdatedAt.HasValue)
            {
                return sort.UpdatedAt.Value == SortOrder.Ascending
                    ? query.OrderBy(keySelector: cv => cv.UpdatedAt)
                    : query.OrderByDescending(keySelector: cv => cv.UpdatedAt);
            }

            return query;
        }
    }
}