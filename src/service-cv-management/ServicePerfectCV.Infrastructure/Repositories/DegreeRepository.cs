using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Degree.Requests;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class DegreeRepository : CrudRepositoryBase<Degree, Guid>, IDegreeRepository
    {
        public DegreeRepository(ApplicationDbContext context) : base(context: context)
        {
        }

        public async Task<IEnumerable<Degree>> SearchByNameAsync(DegreeQuery query)
        {
            var queryable = _context.Degrees
                 .AsNoTracking()
                 .Where(d => d.Name.Contains(query.SearchTerm ?? string.Empty) || d.Code.Contains(query.SearchTerm ?? string.Empty));

            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable;
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        private static IQueryable<Degree> ApplySort(IQueryable<Degree> query, DegreeSort degreeSort)
        {
            if (degreeSort.Name.HasValue)
            {
                return degreeSort.Name.Value == SortOrder.Ascending
                    ? query.OrderBy(d => d.Name)
                    : query.OrderByDescending(d => d.Name);
            }

            if (degreeSort.Code.HasValue)
            {
                return degreeSort.Code.Value == SortOrder.Ascending
                    ? query.OrderBy(d => d.Code)
                    : query.OrderByDescending(d => d.Code);
            }

            return query;
        }
    }
}
