using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class EducationRepository : CrudRepositoryBase<Education, Guid>, IEducationRepository
    {
        public EducationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Education?> GetByIdAndCVIdAndUserIdAsync(Guid educationId, Guid cvId, Guid userId)
        {
            return await _context.Educations
                .AsNoTracking()
                .Include(e => e.CV)
                .FirstOrDefaultAsync(e =>
                    e.Id == educationId && e.CVId == cvId && e.CV.UserId == userId && e.DeletedAt == null);
        }

        public async Task<IEnumerable<Education>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, EducationQuery query)
        {
            var queryable = _context.Educations
                .AsNoTracking()
                .Include(e => e.CV)
                .Where(e => e.CVId == cvId && e.CV.UserId == userId && e.DeletedAt == null);

            queryable = query.Sort != null ? ApplySort(queryable: queryable, sort: query.Sort) : queryable;

            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        private static IQueryable<Education> ApplySort(IQueryable<Education> queryable, EducationSort sort)
        {
            if (sort.StartDate.HasValue)
            {
                return sort.StartDate.Value == SortOrder.Ascending
                    ? queryable.OrderBy(keySelector: e => e.StartDate)
                    : queryable.OrderByDescending(keySelector: e => e.StartDate);
            }

            return queryable;
        }
    }
}