using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ServicePerfectCV.WebApi.Controllers;

[ApiController]
[Route("api/ai")]
public sealed class AiController : ControllerBase
{
    private readonly IAIOrchestrator _orchestrator;
    private readonly IJobStore _jobStore;
    private readonly IJobProcessingService _jobProcessingService;

    public AiController(
        IAIOrchestrator orchestrator,
        IJobStore jobStore,
        IJobProcessingService jobProcessingService)
    {
        _orchestrator = orchestrator;
        _jobStore = jobStore;
        _jobProcessingService = jobProcessingService;
    }

    public sealed record ReviewRequest(string Cv, string Jd);

    [HttpPost("cv/review")]
    public IActionResult ReviewCv([FromBody] ReviewRequest req, CancellationToken ct)
    {
        // Create a new job
        var jobId = _jobStore.CreateJob(req.Cv, req.Jd);

        // Start background processing
        _ = Task.Run(() => _jobProcessingService.ProcessReviewJobAsync(jobId, req.Cv, req.Jd, ct), ct);

        // Return job ID immediately
        return Ok(new { jobId });
    }

    [HttpGet("cv/review/{jobId}")]
    public IActionResult GetReviewResult(string jobId)
    {
        var job = _jobStore.GetJob(jobId);
        if (job == null)
        {
            return NotFound(new { message = "Job not found" });
        }

        return Ok(new
        {
            jobId = job.JobId,
            status = job.Status.ToString(),
            result = job.Result,
            error = job.Error,
            createdAt = job.CreatedAt,
            completedAt = job.CompletedAt
        });
    }
}
