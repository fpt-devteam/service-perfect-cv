using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Common;
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
        public async Task<FilterView<CV>> GetByUserIdAsync(CVQuery query, Guid userId)
        {
            var baseQuery = _context.CVs
                .AsNoTracking()
                .Where(cv => cv.UserId == userId && cv.DeletedAt == null);

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                baseQuery = baseQuery.Where(cv => cv.Title.Contains(query.SearchTerm));
            }

            var sorted = query.Sort != null ? ApplySort(baseQuery, query.Sort) : baseQuery.OrderBy(cv => cv.UpdatedAt ?? cv.CreatedAt);
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

        public Task<CV?> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            return _context.CVs
                .AsNoTracking()
                .FirstOrDefaultAsync(cv => cv.Id == cvId && cv.UserId == userId && cv.DeletedAt == null);
        }

        public async Task<CV?> GetFullContentByCVIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            return await _context.CVs
                .Where(c => c.Id == cvId && c.UserId == userId && c.DeletedAt == null)
                .Include(c => c.Contact)
                .Include(c => c.Summary)
                .Include(c => c.Skills.Where(skill => skill.DeletedAt == null))
                .Include(c => c.Educations.Where(education => education.DeletedAt == null))
                .Include(c => c.Experiences.Where(experience => experience.DeletedAt == null))
                    .ThenInclude(e => e.EmploymentType)
                .Include(c => c.Projects.Where(project => project.DeletedAt == null))
                .Include(c => c.Certifications.Where(certification => certification.DeletedAt == null))
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteByCVIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            var cv = await _context.CVs
                .FirstOrDefaultAsync(cv => cv.Id == cvId && cv.UserId == userId && cv.DeletedAt == null);

            if (cv == null) return false;

            cv.DeletedAt = DateTime.UtcNow;
            _context.CVs.Update(cv);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}