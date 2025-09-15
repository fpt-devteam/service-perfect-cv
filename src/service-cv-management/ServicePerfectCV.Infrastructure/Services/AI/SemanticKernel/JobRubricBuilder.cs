using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Constants;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public class JobRubricBuilder : IJobRubricBuilder

    {
        private readonly ILogger<JobRubricBuilder> _logger;
        private readonly Kernel _kernel;
        private readonly SemanticKernelOptions _options;


        public JobRubricBuilder(ILogger<JobRubricBuilder> logger, Kernel kernel, IOptions<SemanticKernelOptions> options)
        {
            _logger = logger;
            _kernel = kernel;
            _options = options.Value;
        }
        public async Task<JobRubric> BuildJobRubricAsync(JobDescription jd, CancellationToken ct = default)
        {
            var schema = JobRubricSchemaManager.GetJobRubricSchema();
            _logger.LogInformation("Job Rubric Schema: {Schema}", schema);
            var rubricSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = _options.Temperature,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "CvRubric",
                    jsonSchema: BinaryData.FromString(data: JobRubricSchemaManager.GetJobRubricSchema()),
                    jsonSchemaIsStrict: true
                ),
                MaxTokens = _options.MaxTokens
            };
            var rubricBuilderFn = _kernel.CreateFunctionFromPrompt(
                promptTemplate: PromptManager.GetPrompt(promptType: PromptType.JobRubric),
                executionSettings: rubricSettings
            );
            var rubricResult = await _kernel.InvokeAsync(rubricBuilderFn, new KernelArguments
            {
                ["title"] = PromptSanitizeHelper.SanitizeInput(jd.Title),
                ["level"] = PromptSanitizeHelper.SanitizeInput(jd.Level),
                ["requirements"] = string.Join("\n• ", jd.Requirements.Select(PromptSanitizeHelper.SanitizeInput))
            }, ct);
            var rubricResultJson = rubricResult.ToString();
            _logger.LogInformation("Rubric JSON: {RubricJson}", rubricResultJson);
            try
            {
                return JsonHelper.Deserialize<JobRubric>(rubricResultJson ?? string.Empty) ?? new JobRubric();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning("Failed to parse rubric JSON: {Error}", ex.Message);
                return new JobRubric();
            }
        }

        public Dictionary<string, List<string>> GetFallbackRubric(JobDescription jd)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<string>> GetSystemRubricForLevel(string level)
        {
            throw new NotImplementedException();
        }
    }
}
