using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Interfaces.Jobs
{
    public interface IJobRepository
    {
        Task CreateAsync(Job job, CancellationToken cancellationToken);
        Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
