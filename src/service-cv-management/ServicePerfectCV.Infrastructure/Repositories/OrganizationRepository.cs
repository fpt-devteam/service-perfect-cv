using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Organization.Requests;
using ServicePerfectCV.Application.Interfaces;
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
    public class OrganizationRepository : CrudRepositoryBase<Organization, Guid>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Organization>> SearchByNameAsync(OrganizationQuery query)
        {
            var queryable = _context.Organizations
                .AsNoTracking()
                .Where(o => o.OrganizationType == query.OrganizationType && o.Name.Contains(query.SearchTerm ?? string.Empty));
            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable;
            queryable = queryable.Skip(query.Offset).Take(query.Limit);
            return await queryable.ToListAsync();
        }
        private static IQueryable<Organization> ApplySort(IQueryable<Organization> query, OrganizationSort organizationSort)
        {
            if (organizationSort.Name.HasValue)
            {
                return organizationSort.Name.Value == SortOrder.Ascending
                    ? query.OrderBy(o => o.Name)
                    : query.OrderByDescending(o => o.Name);
            }
            return query;

        }
    }
}
