using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
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
    public class SkillRepository : CrudRepositoryBase<Skill, Guid>, ISkillRepository
    {
        public SkillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Skill>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, SkillQuery query)
        {
            var queryable = _context.Skills
                .AsNoTracking()
                .Include(s => s.CV)
                .Include(s => s.Category)
                .Where(s => s.CVId == cvId && s.CV.UserId == userId &&
                            s.DeletedAt == null);

            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable;
            queryable = queryable.Skip(query.Offset).Take(query.Limit);
            return await queryable.ToListAsync();
        }

        public async Task<Skill?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId)
        {
            return await _context.Skills
                .AsNoTracking()
                .Include(s => s.CV)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => 
                    s.CVId == cvId && s.CV.UserId == userId && s.Id == id && s.DeletedAt == null);
        }

        private static IQueryable<Skill> ApplySort(IQueryable<Skill> query, SkillSort skillSort)
        {
            if (skillSort.Category.HasValue)
            {
                return skillSort.Category.Value == SortOrder.Ascending
                    ? query.OrderBy(s => s.Category.Name)
                    : query.OrderByDescending(s => s.Category.Name);
            }

            return query.OrderBy(s => s.Category.Name);
        }

        public async Task<Skill?> GetByIdWithCategoryAsync(Guid id)
        {
            return await _context.Skills
                .Include(s => s.Category)
                .Where(s => s.Id == id && s.DeletedAt == null)
                .FirstOrDefaultAsync();
        }
    }
}
