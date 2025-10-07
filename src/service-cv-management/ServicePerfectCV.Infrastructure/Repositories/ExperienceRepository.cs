using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class ExperienceRepository : CrudRepositoryBase<Experience, Guid>, IExperienceRepository
    {
        public ExperienceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Experience>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            return await _context.Experiences
                .AsNoTracking()
                .Include(e => e.CV)
                .Include(e => e.EmploymentType)
                .Where(e => e.CVId == cvId && e.CV.UserId == userId &&
                            e.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Experience?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId)
        {
            return await _context.Experiences
                .AsNoTracking()
                .Include(e => e.CV)
                .Include(e => e.EmploymentType)
                .FirstOrDefaultAsync(e =>
                    e.CVId == cvId && e.CV.UserId == userId && e.Id == id && e.DeletedAt == null);
        }
    }
}