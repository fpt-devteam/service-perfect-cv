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

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class CVRepository(ApplicationDbContext context) : CrudRepositoryBase<CV, Guid>(context), ICVRepository
    {
        public async Task<IEnumerable<CV>> GetByUserIdAsync(CVQuery query, Guid userId)
        {
            var queryable = _context.CVs
                .AsNoTracking()
                .Where(cv => cv.UserId == userId);
            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable;
            queryable = queryable.Skip(query.Offset).Take(query.Limit);
            return await queryable.ToListAsync();
        }
        private static IQueryable<CV> ApplySort(IQueryable<CV> query, CVSort sort)
        {
            if (sort.UpdatedAt.HasValue)
            {
                return sort.UpdatedAt.Value == SortOrder.Ascending
                    ? query.OrderBy(keySelector: cv => cv.CreatedAt)
                    : query.OrderByDescending(keySelector: cv => cv.CreatedAt);
            }

            return query;
        }
    } 
}