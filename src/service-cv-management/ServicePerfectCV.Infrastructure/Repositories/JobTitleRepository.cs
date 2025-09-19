using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.JobTitle.Requests;
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
    public class JobTitleRepository : CrudRepositoryBase<JobTitle, Guid>, IJobTitleRepository
    {
        public JobTitleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<JobTitle>> SearchByNameAsync(JobTitleQuery query)
        {
            var queryable = _context.JobTitles.AsNoTracking()
                .Where(jt => jt.DeletedAt == null && jt.Name.Contains(query.SearchTerm ?? string.Empty));
            queryable = ApplySort(queryable, query.Sort);
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        public async Task<JobTitle?> GetByNameAsync(string name)
        {
            return await _context.JobTitles.FirstOrDefaultAsync(jt => jt.Name == name);
        }

        private static IQueryable<JobTitle> ApplySort(IQueryable<JobTitle> queryable, JobTitleSort sort)
        {
            if (sort.Name.HasValue)
            {
                return sort.Name.Value == Domain.Constants.SortOrder.Ascending
                    ? queryable.OrderBy(jt => jt.Name)
                    : queryable.OrderByDescending(jt => jt.Name);
            }

            return queryable;
        }
    }
}