using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Jobs.Requests;
using ServicePerfectCV.Application.DTOs.Jobs.Responses;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController(JobService jobService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
        {
            JsonDocument inputDocument;
            try
            {
                inputDocument = ParseInput(request.InputJson);
            }
            catch (JsonException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var job = await jobService.CreateAsync(request.JobType, inputDocument, request.Priority, cancellationToken);
            var response = MapToJobResultResponse(job);
            return Ok(response);
        }

        [HttpGet("{id:guid}/result")]
        public async Task<IActionResult> GetResultAsync(Guid id, CancellationToken cancellationToken)
        {
            var job = await jobService.GetAsync(id, cancellationToken);
            if (job == null)
                return NotFound();

            var result = MapToJobResultResponse(job);
            return Ok(result);
        }

        private static JsonDocument ParseInput(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                var raw = element.GetString();
                if (string.IsNullOrWhiteSpace(raw))
                    throw new JsonException("inputJson cannot be empty.");

                return JsonDocument.Parse(raw);
            }

            return JsonDocument.Parse(element.GetRawText());
        }

        private static JobResultResponse MapToJobResultResponse(Job job)
        {
            JsonElement? output = job.Output?.RootElement.Clone();

            return new JobResultResponse
            {
                Id = job.Id,
                Type = job.Type.ToString(),
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                InputJson = job.Input.RootElement.Clone(),
                OutputJson = output,
                ErrorCode = job.ErrorCode,
                ErrorMessage = job.ErrorMessage
            };
        }
    }
}
