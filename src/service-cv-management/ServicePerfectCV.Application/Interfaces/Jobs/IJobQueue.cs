using System;
using System.Threading;
using System.Threading.Tasks;
using ServicePerfectCV.Application.Services.Jobs;

namespace ServicePerfectCV.Application.Interfaces.Jobs
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(QueuedJob job, CancellationToken cancellationToken);
        ValueTask<QueuedJob?> DequeueAsync(CancellationToken cancellationToken);
    }
}
