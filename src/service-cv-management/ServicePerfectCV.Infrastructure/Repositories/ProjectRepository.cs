using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Project.Requests;
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
    public class ProjectRepository : CrudRepositoryBase<Project, Guid>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, ProjectQuery query)
        {
            var queryable = _context.Projects
                .AsNoTracking()
                .Include(p => p.CV)
                .Where(p => p.CVId == cvId && p.CV.UserId == userId &&
                            p.DeletedAt == null);
            //1. apply filter 
            //2. apply sort 
            //3. apply pagination
            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable;
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        public async Task<Project?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.CV)
                .FirstOrDefaultAsync(p =>
                    p.CVId == cvId && p.CV.UserId == userId && p.Id == id && p.DeletedAt == null);
        }

        private static IQueryable<Project> ApplySort(IQueryable<Project> query, ProjectSort projectSort)
        {
            if (projectSort.StartDate.HasValue)
            {
                return projectSort.StartDate.Value == SortOrder.Ascending
                    ? query.OrderBy(keySelector: p => p.StartDate)
                    : query.OrderByDescending(keySelector: p => p.StartDate);
            }

            return query;
        }
    }
}