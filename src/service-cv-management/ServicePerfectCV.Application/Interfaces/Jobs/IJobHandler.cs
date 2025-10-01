using System.Threading;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Job;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.Interfaces.Jobs
{
    public interface IJobHandler
    {
        JobType JobType { get; }
        Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken);
    }
}
