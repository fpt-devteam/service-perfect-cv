using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Job;
using ServicePerfectCV.Application.Interfaces.Jobs;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class InMemoryJobQueue : IJobQueue
    {
        private readonly PriorityQueue<QueuedJob, (DateTimeOffset, int, long)> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);
        private long _sequence;

        public ValueTask EnqueueAsync(QueuedJob job, CancellationToken cancellationToken)
        {
            EnqueueInternal(job);
            return ValueTask.CompletedTask;
        }

        public async ValueTask<QueuedJob?> DequeueAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                QueuedJob? readyJob = null;
                TimeSpan? delay = null;

                lock (_queue)
                {
                    if (_queue.TryPeek(out var job, out _))
                    {
                        var now = DateTimeOffset.UtcNow;
                        if (job.VisibleAt <= now)
                        {
                            _queue.Dequeue();
                            readyJob = job;
                        }
                        else
                        {
                            delay = job.VisibleAt - now;
                        }
                    }
                }

                if (readyJob != null)
                    return readyJob;

                if (delay.HasValue)
                {
                    await Task.Delay(delay.Value, cancellationToken);
                    continue;
                }

                await _signal.WaitAsync(cancellationToken);
            }
        }


        private void EnqueueInternal(QueuedJob job)
        {
            var normalized = job.VisibleAt > DateTimeOffset.UtcNow
                ? job
                : job with { VisibleAt = DateTimeOffset.UtcNow };

            lock (_queue)
            {
                var priority = (normalized.VisibleAt, -normalized.Priority, Interlocked.Increment(ref _sequence));
                _queue.Enqueue(normalized, priority);
            }

            _signal.Release();
        }
    }
}
