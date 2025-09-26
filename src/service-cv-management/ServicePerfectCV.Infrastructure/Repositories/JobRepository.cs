using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public sealed class JobRepository : CrudRepositoryBase<Job, Guid>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Job job, CancellationToken cancellationToken)
        {
            await _context.Jobs.AddAsync(job, cancellationToken);
        }

        public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Jobs.FirstOrDefaultAsync(job => job.Id == id, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
