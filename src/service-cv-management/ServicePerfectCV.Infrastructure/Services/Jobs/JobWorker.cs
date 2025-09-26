using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class JobWorker(
        IJobQueue jobQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<JobWorker> logger) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                QueuedJob? queuedJob = await DequeueJobAsync(stoppingToken);
                if (queuedJob == null)
                    continue;

                using var scope = scopeFactory.CreateScope();
                var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                var jobRouter = scope.ServiceProvider.GetRequiredService<JobRouter>();

                Job? job = await jobRepository.GetByIdAsync(queuedJob.JobId, stoppingToken);
                if (job == null) continue;

                IJobHandler? handler = ResolveHandler(jobRouter, job, jobRepository, stoppingToken);
                if (handler == null || job.IsTerminal || job.Status == JobStatus.Canceled) continue;

                try
                {
                    job.MarkRunning(DateTimeOffset.UtcNow);
                    await jobRepository.SaveChangesAsync(stoppingToken);

                    var result = await handler.HandleAsync(job, stoppingToken);

                    if (result.Succeeded && result.Output != null)
                    {
                        job.MarkSucceeded(result.Output, DateTimeOffset.UtcNow);
                        await jobRepository.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    job.MarkFailed(result.ErrorCode, result.ErrorMessage, DateTimeOffset.UtcNow);
                    await jobRepository.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception while processing job {JobId}", job.Id);

                    job.MarkFailed("job.unhandled", ex.Message, DateTimeOffset.UtcNow);
                    await jobRepository.SaveChangesAsync(stoppingToken);
                }
            }
        }

        private async Task<QueuedJob?> DequeueJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                return await jobQueue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private IJobHandler? ResolveHandler(
            JobRouter jobRouter,
            Job job,
            IJobRepository jobRepository,
            CancellationToken ct)
        {
            try
            {
                return jobRouter.Resolve(job.Type);
            }
            catch (InvalidOperationException ex)
            {
                job.MarkFailed("job.handler_not_found", ex.Message, DateTimeOffset.UtcNow);
                jobRepository.SaveChangesAsync(ct).Wait();
                return null;
            }
        }
    }
}
