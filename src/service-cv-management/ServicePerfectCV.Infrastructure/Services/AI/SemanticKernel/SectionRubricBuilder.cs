using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Constants;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public class SectionRubricBuilder : ISectionRubricBuilder

    {
        private readonly ILogger<SectionRubricBuilder> _logger;
        private readonly Kernel _kernel;
        private readonly SemanticKernelOptions _options;
        private readonly IJsonHelper _jsonHelper;

        public SectionRubricBuilder(ILogger<SectionRubricBuilder> logger, Kernel kernel, IOptions<SemanticKernelOptions> options, IJsonHelper jsonHelper)
        {
            _logger = logger;
            _kernel = kernel;
            _options = options.Value;
            _jsonHelper = jsonHelper;
        }
        public async Task<SectionRubricDictionary> BuildSectionRubricsAsync(JobDescription jd, CancellationToken ct = default)
        {
            var schema = _jsonHelper.GenerateJsonSchema<SectionRubricDictionary>();
            _logger.LogDebug("Job Rubric Schema: {Schema}", schema);

            var rubricSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = _options.Temperature,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "CvRubric",
                    jsonSchema: BinaryData.FromString(data: schema),
                    jsonSchemaIsStrict: true
                ),
                MaxTokens = _options.MaxTokens
            };

            var rubricBuilderFn = _kernel.CreateFunctionFromPrompt(
                promptTemplate: PromptManager.GetPrompt(promptType: PromptType.SectionRubricBuilding),
                executionSettings: rubricSettings
            );
            _logger.LogDebug("Start prompt to {Provider} model: {Model}", _options.Provider, _options.OllamaAIModel);
            try
            {
                var rubricResult = await _kernel.InvokeAsync(rubricBuilderFn, new KernelArguments
                {
                    ["title"] = PromptSanitizeHelper.SanitizeInput(jd.Title),
                    ["level"] = PromptSanitizeHelper.SanitizeInput(jd.Level),
                    ["requirements"] = _jsonHelper.Serialize(jd.Requirements),
                    ["rubricSchema"] = schema
                }, ct);

                var rubricResultJson = rubricResult.ToString();
                _logger.LogDebug("Rubric JSON: {RubricJson}", rubricResultJson);
                return _jsonHelper.Deserialize<SectionRubricDictionary>(rubricResultJson ?? string.Empty) ?? new SectionRubricDictionary();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Failed to parse rubric JSON: {Error}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during AI prompt execution: {Error}", ex.Message);
                throw;
            }
        }
    }
}