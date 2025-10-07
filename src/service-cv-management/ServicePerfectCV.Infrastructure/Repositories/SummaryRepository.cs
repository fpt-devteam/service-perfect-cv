using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class SummaryRepository : CrudRepositoryBase<Summary, Guid>, ISummaryRepository
    {
        public SummaryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Summary?> GetByCVIdAsync(Guid cvId)
        {
            return await _context.Summaries
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.CVId == cvId);
        }
    }
}