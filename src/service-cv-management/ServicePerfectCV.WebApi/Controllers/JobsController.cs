using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateAsync(CancellationToken cancellationToken)
        {
            var json = @"
            {
            ""JobDescriptionId"": ""56ee5f10-2686-4f59-8f56-a968928aef5c"",
            ""Title"": ""Software Engineer Intern - AI/Backend Systems"",
            ""CompanyName"": ""FinTech Fraud Detection Solutions"",
            ""Responsibility"": ""Participate in the full life cycle of software development for backend systems. Collaborate across teams to research and analyze requirements for new features, focusing on integrating AI and ML models into the fraud detection pipeline. Implement well-tested code and work with QA to ensure high-quality, reliable, and scalable deliverables. Solve production problems for large-scale systems, optimize performance using data-driven insights and AI-powered monitoring tools. Contribute to AI/ML-driven features such as anomaly detection, behavioral analysis, and real-time fraud prevention. Provide active input to improve the product and processes by leveraging AI for efficiency."",
            ""Qualification"": ""Pursuing or recently graduated with a Bachelor's in Software Engineering, Computer Science, or a related field. Solid knowledge of programming fundamentals, data structures, and algorithms. Ability to tackle new and complex problems in data-rich environments. Capable of independently assessing issues and taking action. High integrity and ability to maintain confidentiality of sensitive information. Strong written and verbal communication skills in English. Interest or foundational knowledge in machine learning, data science, or AI is a strong plus.""
            }";
            JsonElement inputJson = JsonDocument.Parse(json).RootElement;
            JobType jobType = JobType.BuildCvSectionRubric;
            int priority = 0;

            JsonDocument inputDocument;
            try
            {
                inputDocument = ParseInput(inputJson);
            }
            catch (JsonException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var job = await jobService.CreateAsync(jobType, inputDocument, priority, cancellationToken);
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

        private static object MapToJobResultResponse(Job job)
        {
            JsonElement? output = job.Output?.RootElement.Clone();

            return new
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
