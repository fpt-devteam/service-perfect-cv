using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models.Jobs;

namespace ServicePerfectCV.WebAdmin.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(
            ApplicationDbContext context,
            JobService jobService,
            ILogger<JobsController> logger)
        {
            _context = context;
            _jobService = jobService;
            _logger = logger;
        }

        // GET: Jobs
        public async Task<IActionResult> Index(JobType? type, JobStatus? status, int page = 1, int pageSize = 50)
        {
            ViewData["CurrentType"] = type;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentPage"] = page;

            var query = _context.Jobs
                .Where(j => j.DeletedAt == null)
                .AsQueryable();

            // Apply filters
            if (type.HasValue)
            {
                query = query.Where(j => j.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(j => j.Status == status.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobListViewModel
                {
                    Id = j.Id,
                    Type = j.Type,
                    Status = j.Status,
                    CreatedAt = j.CreatedAt,
                    StartedAt = j.StartedAt,
                    CompletedAt = j.CompletedAt,
                    Priority = j.Priority,
                    ErrorCode = j.ErrorCode,
                    ErrorMessage = j.ErrorMessage
                })
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            // Get statistics
            var stats = await _context.Jobs
                .Where(j => j.DeletedAt == null)
                .GroupBy(j => j.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.QueuedCount = stats.FirstOrDefault(s => s.Status == JobStatus.Queued)?.Count ?? 0;
            ViewBag.RunningCount = stats.FirstOrDefault(s => s.Status == JobStatus.Running)?.Count ?? 0;
            ViewBag.SucceededCount = stats.FirstOrDefault(s => s.Status == JobStatus.Succeeded)?.Count ?? 0;
            ViewBag.FailedCount = stats.FirstOrDefault(s => s.Status == JobStatus.Failed)?.Count ?? 0;
            ViewBag.CanceledCount = stats.FirstOrDefault(s => s.Status == JobStatus.Canceled)?.Count ?? 0;

            return View(jobs);
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.DeletedAt == null);

            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new JobDetailViewModel
            {
                Id = job.Id,
                Type = job.Type,
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                Priority = job.Priority,
                ErrorCode = job.ErrorCode,
                ErrorMessage = job.ErrorMessage,
                InputJson = job.Input.RootElement.GetRawText(),
                OutputJson = job.Output?.RootElement.GetRawText()
            };

            return View(viewModel);
        }

        // POST: Jobs/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null || job.DeletedAt != null)
                {
                    TempData["ErrorMessage"] = "Job not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (job.IsTerminal)
                {
                    TempData["InfoMessage"] = "Job has already completed and cannot be canceled.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                job.MarkCanceled(DateTimeOffset.UtcNow);
                job.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} canceled by admin {AdminEmail}", id, User.Identity?.Name);
                TempData["SuccessMessage"] = "Job canceled successfully.";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling job {JobId}", id);
                TempData["ErrorMessage"] = "Unable to cancel job. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Jobs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null || job.DeletedAt != null)
                {
                    TempData["ErrorMessage"] = "Job not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Soft delete
                job.DeletedAt = DateTimeOffset.UtcNow;
                job.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} soft deleted by admin {AdminEmail}", id, User.Identity?.Name);
                TempData["SuccessMessage"] = "Job deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                TempData["ErrorMessage"] = "Unable to delete job. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Jobs/RetryFailed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetryFailed(Guid id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null || job.DeletedAt != null)
                {
                    TempData["ErrorMessage"] = "Job not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (job.Status != JobStatus.Failed)
                {
                    TempData["InfoMessage"] = "Only failed jobs can be retried.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Create a new job with the same input
                var newJob = Domain.Entities.Job.Create(
                    Guid.NewGuid(),
                    job.Type,
                    job.Input,
                    job.Priority,
                    DateTimeOffset.UtcNow
                );

                _context.Jobs.Add(newJob);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} retried as {NewJobId} by admin {AdminEmail}",
                    id, newJob.Id, User.Identity?.Name);
                TempData["SuccessMessage"] = $"Job retry queued successfully. New Job ID: {newJob.Id}";

                return RedirectToAction(nameof(Details), new { id = newJob.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying job {JobId}", id);
                TempData["ErrorMessage"] = "Unable to retry job. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Jobs/ClearCompleted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCompleted()
        {
            try
            {
                var completedJobs = await _context.Jobs
                    .Where(j => j.DeletedAt == null &&
                        (j.Status == JobStatus.Succeeded || j.Status == JobStatus.Canceled) &&
                        j.CompletedAt < DateTimeOffset.UtcNow.AddDays(-7))
                    .ToListAsync();

                foreach (var job in completedJobs)
                {
                    job.DeletedAt = DateTimeOffset.UtcNow;
                    job.UpdatedAt = DateTimeOffset.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleared {Count} completed jobs by admin {AdminEmail}",
                    completedJobs.Count, User.Identity?.Name);
                TempData["SuccessMessage"] = $"Successfully cleared {completedJobs.Count} completed jobs older than 7 days.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing completed jobs");
                TempData["ErrorMessage"] = "Unable to clear completed jobs. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

