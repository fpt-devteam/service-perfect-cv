using ServicePerfectCV.Application.DTOs.Job;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed class JobService(IJobRepository jobRepository, IJobQueue jobQueue)
    {
        public async Task<Job> CreateAsync(JobType jobType, JsonDocument input, int priority, CancellationToken cancellationToken)
        {
            var job = Job.Create(
                id: Guid.NewGuid(),
                type: jobType,
                input: input,
                priority: priority,
                createdAt: DateTimeOffset.UtcNow);

            await jobRepository.CreateAsync(job, cancellationToken);
            await jobRepository.SaveChangesAsync(cancellationToken);

            QueuedJob queuedJob = new(
                JobId: job.Id,
                JobType: job.Type,
                Priority: job.Priority,
                VisibleAt: DateTimeOffset.UtcNow
            );

            await jobQueue.EnqueueAsync(queuedJob, cancellationToken);
            return job;
        }

        public Task<Job?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return jobRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Job?> CancelAsync(Guid id, CancellationToken cancellationToken)
        {
            var job = await jobRepository.GetByIdAsync(id, cancellationToken);
            if (job == null)
                return null;

            if (!job.IsTerminal)
            {
                job.MarkCanceled(DateTimeOffset.UtcNow);
                await jobRepository.SaveChangesAsync(cancellationToken);
            }

            return job;
        }
    }
}
