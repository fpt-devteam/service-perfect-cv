using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using System.Collections.Concurrent;

namespace ServicePerfectCV.Application.Services;

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public class Job<T>
{
    public string JobId { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public T? Result { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public interface IJobStore<T>
{
    string CreateJob();
    Job<T>? GetJob(string jobId);
    void UpdateJob(string jobId, JobStatus status, T? result = default, string? error = null);
    void CompleteJob(string jobId, T result);
    void FailJob(string jobId, string error);
}

public sealed class InMemoryJobStore<T> : IJobStore<T>
{
    private readonly ConcurrentDictionary<string, Job<T>> _jobs = new();

    public string CreateJob()
    {
        var jobId = Guid.NewGuid().ToString("N");
        var job = new Job<T>
        {
            JobId = jobId,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _jobs[jobId] = job;
        return jobId;
    }

    public Job<T>? GetJob(string jobId)
    {
        return _jobs.TryGetValue(jobId, out var job) ? job : null;
    }

    public void UpdateJob(string jobId, JobStatus status, T? result, string? error = null)
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

    public void CompleteJob(string jobId, T result)
    {
        UpdateJob(jobId, JobStatus.Completed, result);
    }

    public void FailJob(string jobId, string error)
    {
        UpdateJob(jobId, JobStatus.Failed, default, error);
    }
}

public interface IJobProcessingService
{
    Task ProcessScoreCvSectionJobAsync(string jobId, CvEntity cv, SectionRubricDictionary sectionRubric, CancellationToken ct = default);
}

public sealed class JobProcessingService : IJobProcessingService
{
    private readonly IAIOrchestrator _orchestrator;
    private readonly IJobStore<CvAnalysisFinalOutput> _jobStore;
    private readonly ILogger<JobProcessingService> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<JobProcessingService>();

    public JobProcessingService(IAIOrchestrator orchestrator, IJobStore<CvAnalysisFinalOutput> jobStore)
    {
        _orchestrator = orchestrator;
        _jobStore = jobStore;
    }

    public async Task ProcessScoreCvSectionJobAsync(string jobId, CvEntity cv, SectionRubricDictionary sectionRubric, CancellationToken ct = default)
    {
        try
        {
            // Update job status to processing
            _jobStore.UpdateJob(jobId: jobId, status: JobStatus.Processing);

            _logger.LogInformation("Starting CV analysis job {JobId}", jobId);

            var result = await _orchestrator.ScoreCvSectionsAgainstRubricAsync(
                cv: cv,
                rubricDictionary: sectionRubric,
                ct: ct);

            _logger.LogInformation("Completed CV analysis job {JobId}", jobId);

            // Complete the job with the result
            _jobStore.CompleteJob(jobId: jobId, result: result);
        }
        catch (Exception ex)
        {
            // Fail the job with error message
            _jobStore.FailJob(jobId: jobId, error: ex.Message);
            _logger.LogError(ex, "Failed CV analysis job {JobId}", jobId);
            _logger.LogError("Exception: {Message}", ex.Message);
        }
    }
}
