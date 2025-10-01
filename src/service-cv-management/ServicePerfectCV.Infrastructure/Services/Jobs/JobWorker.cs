using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.DTOs.Job;
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
                    logger.LogInformation("Starting job {JobId} of type {JobType}", job.Id, job.Type);
                    job.MarkRunning(DateTimeOffset.UtcNow);
                    await jobRepository.SaveChangesAsync(stoppingToken);

                    using var jobTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    var jobTimeout = GetJobTimeout(job.Type);
                    jobTokenSource.CancelAfter(jobTimeout);

                    var result = await handler.HandleAsync(job, jobTokenSource.Token);
                    logger.LogInformation("Job {JobId} of type {JobType} completed with status: isSuccess {Succeeded}, isFailed {Failed}", job.Id, job.Type, result.Succeeded, result.ErrorMessage);

                    if (result.Succeeded && result.Output != null)
                    {
                        job.MarkSucceeded(result.Output, DateTimeOffset.UtcNow);
                    }
                    else
                    {
                        job.MarkFailed(result.ErrorCode, result.ErrorMessage, DateTimeOffset.UtcNow);
                    }

                    await jobRepository.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException ex)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        job.MarkCanceled(DateTimeOffset.UtcNow);
                        await jobRepository.SaveChangesAsync(stoppingToken);
                        logger.LogWarning("Job {JobId} of type {JobType} was canceled due to service shutdown: {Reason}", job.Id, job.Type, ex.Message);
                        break;
                    }
                    else
                    {
                        job.MarkFailed("job.timeout", $"Job timed out after {GetJobTimeout(job.Type)}: {ex.Message}", DateTimeOffset.UtcNow);
                        await jobRepository.SaveChangesAsync(stoppingToken);
                        logger.LogWarning("Job {JobId} of type {JobType} timed out after {Timeout}: {Reason}",
                            job.Id, job.Type, GetJobTimeout(job.Type), ex.Message);
                    }
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

        private static TimeSpan GetJobTimeout(JobType jobType)
        {
            return jobType switch
            {
                JobType.BuildCvSectionRubric => TimeSpan.FromMinutes(15),
                JobType.ScoreCV => TimeSpan.FromMinutes(12),
                _ => TimeSpan.FromMinutes(8)
            };
        }
    }
}
