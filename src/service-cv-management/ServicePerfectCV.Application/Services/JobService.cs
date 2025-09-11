using System.Collections.Concurrent;
using ServicePerfectCV.Application.Interfaces.AI;

namespace ServicePerfectCV.Application.Services;

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public class JobResult
{
    public string JobId { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public string? Result { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public interface IJobStore
{
    string CreateJob(string cvText, string jdText);
    JobResult? GetJob(string jobId);
    void UpdateJob(string jobId, JobStatus status, string? result = null, string? error = null);
    void CompleteJob(string jobId, string result);
    void FailJob(string jobId, string error);
}

public sealed class InMemoryJobStore : IJobStore
{
    private readonly ConcurrentDictionary<string, JobResult> _jobs = new();

    public string CreateJob(string cvText, string jdText)
    {
        var jobId = Guid.NewGuid().ToString("N");
        var job = new JobResult
        {
            JobId = jobId,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _jobs[jobId] = job;
        return jobId;
    }

    public JobResult? GetJob(string jobId)
    {
        return _jobs.TryGetValue(jobId, out var job) ? job : null;
    }

    public void UpdateJob(string jobId, JobStatus status, string? result = null, string? error = null)
    {
        if (_jobs.TryGetValue(jobId, out var job))
        {
            job.Status = status;
            if (result != null) job.Result = result;
            if (error != null) job.Error = error;
            if (status == JobStatus.Completed || status == JobStatus.Failed)
            {
                job.CompletedAt = DateTime.UtcNow;
            }
        }
    }

    public void CompleteJob(string jobId, string result)
    {
        UpdateJob(jobId, JobStatus.Completed, result);
    }

    public void FailJob(string jobId, string error)
    {
        UpdateJob(jobId, JobStatus.Failed, null, error);
    }
}

public interface IJobProcessingService
{
    Task ProcessReviewJobAsync(string jobId, string cvText, string jdText, CancellationToken ct = default);
}

public sealed class JobProcessingService : IJobProcessingService
{
    private readonly IAIOrchestrator _orchestrator;
    private readonly IJobStore _jobStore;

    public JobProcessingService(IAIOrchestrator orchestrator, IJobStore jobStore)
    {
        _orchestrator = orchestrator;
        _jobStore = jobStore;
    }

    public async Task ProcessReviewJobAsync(string jobId, string cvText, string jdText, CancellationToken ct = default)
    {
        try
        {
            // Update job status to processing
            _jobStore.UpdateJob(jobId, JobStatus.Processing);

            // Process the AI review
            var result = await _orchestrator.ReviewCvAgainstJdAsync(cvText, jdText, ct);

            // Complete the job with the result
            _jobStore.CompleteJob(jobId, result);
        }
        catch (Exception ex)
        {
            // Fail the job with error message
            _jobStore.FailJob(jobId, ex.Message);
        }
    }
}
