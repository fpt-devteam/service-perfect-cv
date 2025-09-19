using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.EmploymentType.Requests;
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
    public class EmploymentTypeRepository : CrudRepositoryBase<EmploymentType, Guid>, IEmploymentTypeRepository
    {
        public EmploymentTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmploymentType>> SearchByNameAsync(EmploymentTypeQuery query)
        {
            var queryable = _context.EmploymentTypes.AsNoTracking()
                .Where(et => et.DeletedAt == null && et.Name.Contains(query.SearchTerm ?? string.Empty));
            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable.OrderBy(et => et.Name);
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        private static IQueryable<EmploymentType> ApplySort(IQueryable<EmploymentType> queryable, EmploymentTypeSort sort)
        {
            if (sort.Name.HasValue)
            {
                return sort.Name.Value == Domain.Constants.SortOrder.Ascending
                    ? queryable.OrderBy(et => et.Name)
                    : queryable.OrderByDescending(et => et.Name);
            }

            return queryable;
        }
    }
}