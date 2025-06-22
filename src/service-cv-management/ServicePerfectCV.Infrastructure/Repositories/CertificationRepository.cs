using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Certification.Requests;
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
    public class CertificationRepository : CrudRepositoryBase<Certification, Guid>, ICertificationRepository
    {
        public CertificationRepository(ApplicationDbContext context) : base(context: context)
        {
        }

        public async Task<Certification?> GetByIdAndCVIdAndUserIdAsync(Guid certificationId, Guid cvId, Guid userId)
        {
            return await _context.Certifications
                .AsNoTracking()
                .Include(e => e.CV)
                .FirstOrDefaultAsync(e =>
                    e.Id == certificationId && e.CVId == cvId && e.CV.UserId == userId && e.DeletedAt == null);
        }

        public async Task<IEnumerable<Certification>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId,
            CertificationQuery query)
        {
            var queryable = _context.Certifications
                .AsNoTracking()
                .Include(e => e.CV)
                .Where(e => e.CVId == cvId && e.CV.UserId == userId && e.DeletedAt == null);

            queryable = query.Sort != null ? ApplySort(queryable: queryable, sort: query.Sort) : queryable;
            
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        private static IQueryable<Certification> ApplySort(IQueryable<Certification> queryable, CertificationSort sort)
        {
            if (sort.IssuedDate.HasValue)
            {
                return sort.IssuedDate.Value == SortOrder.Ascending
                    ? queryable.OrderBy(keySelector: e => e.IssuedDate)
                    : queryable.OrderByDescending(keySelector: e => e.IssuedDate);
            }

            if (sort.Name.HasValue)
            {
                return sort.Name.Value == SortOrder.Ascending
                    ? queryable.OrderBy(keySelector: e => e.Name)
                    : queryable.OrderByDescending(keySelector: e => e.Name);
            }

            return queryable;
        }
    }
}