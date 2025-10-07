using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.DTOs.JobDescription;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Infrastructure.Constants;
using ServicePerfectCV.Infrastructure.DependencyInjections;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI
{
    public class SectionRubricService : ISectionRubricService

    {
        private readonly ILogger<SectionRubricService> _logger;
        private readonly Kernel _kernel;
        private readonly SemanticKernelSettings _options;
        private readonly IJsonHelper _jsonHelper;

        public SectionRubricService(ILogger<SectionRubricService> logger, Kernel kernel, IOptions<SemanticKernelSettings> options, IJsonHelper jsonHelper)
        {
            _logger = logger;
            _kernel = kernel;
            _options = options.Value;
            _jsonHelper = jsonHelper;
        }
        public async Task<SectionRubricDictionary> BuildSectionRubricsAsync(JobDescriptionRubricInputDto jd, CancellationToken ct = default)
        {
            var schema = CreateRubricJsonSchema();
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
            _logger.LogInformation("Start prompt to {Provider} model: {Model}", _options.Provider, _options.OllamaAIModel);
            try
            {
                var rubricResult = await _kernel.InvokeAsync(rubricBuilderFn, new KernelArguments
                {
                    ["title"] = PromptSanitizeHelper.SanitizeInput(jd.Title),
                    ["company"] = PromptSanitizeHelper.SanitizeInput(jd.CompanyName),
                    ["responsibility"] = PromptSanitizeHelper.SanitizeInput(jd.Responsibility),
                    ["qualification"] = PromptSanitizeHelper.SanitizeInput(jd.Qualification),
                    ["rubricSchema"] = schema
                }, ct);

                var rubricResultJson = rubricResult.ToString();
                _logger.LogInformation("Rubric result JSON: {RubricJson}", rubricResultJson);

                rubricResultJson = CleanJsonResponse(rubricResultJson);

                return _jsonHelper.Deserialize<SectionRubricDictionary>(rubricResultJson ?? string.Empty) ?? new SectionRubricDictionary();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("AI prompt execution was canceled: {Error}", ex.Message);
                throw;
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

        private static string CreateRubricJsonSchema()
        {
            return """
            {
            "$schema": "https://json-schema.org/draft/2020-12/schema",
            "type": "object",
            "description": "SectionRubricDictionary = a map from section name to SectionRubric.",
            "properties": {
                "Contact":        { "$ref": "#/$defs/SectionRubric" },
                "Summary":        { "$ref": "#/$defs/SectionRubric" },
                "Skills":         { "$ref": "#/$defs/SectionRubric" },
                "Experience":     { "$ref": "#/$defs/SectionRubric" },
                "Projects":       { "$ref": "#/$defs/SectionRubric" },
                "Education":      { "$ref": "#/$defs/SectionRubric" },
                "Certifications": { "$ref": "#/$defs/SectionRubric" }
            },
            "additionalProperties": false,
            "$defs": {
                "ScoringScale": {
                "type": "object",
                "properties": {
                    "0": { "type": "string" },
                    "1": { "type": "string" },
                    "2": { "type": "string" },
                    "3": { "type": "string" },
                    "4": { "type": "string" },
                    "5": { "type": "string" }
                },
                "required": ["0", "1", "2", "3", "4", "5"],
                "additionalProperties": false
                },
                "SectionRubricCriteria": {
                "type": "object",
                "properties": {
                    "id":           { "type": "string", "minLength": 1 },
                    "criterion":    { "type": "string", "minLength": 1 },
                    "description":  { "type": "string", "minLength": 1 },
                    "weight0To1":   { "type": "number", "minimum": 0, "maximum": 1 },
                    "scoring":      { "$ref": "#/$defs/ScoringScale" }
                },
                "required": ["id", "criterion", "description", "weight0To1", "scoring"],
                "additionalProperties": false
                },
                "SectionRubric": {
                "type": "object",
                "properties": {
                    "Weight0To1": { "type": "number", "minimum": 0, "maximum": 1 },
                    "Criteria": {
                    "type": "array",
                    "items": { "$ref": "#/$defs/SectionRubricCriteria" },
                    "maxItems": 4
                    }
                },
                "required": ["Weight0To1", "Criteria"],
                "additionalProperties": false
                }
            }
            }
            """;
        }

        private static string CleanJsonResponse(string? jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
                return string.Empty;

            var cleaned = jsonResponse.Trim();

            // Remove markdown code blocks if present
            if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(7); // Remove ```json
            }
            else if (cleaned.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(3); // Remove ```
            }

            if (cleaned.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 3); // Remove closing ```
            }

            return cleaned.Trim();
        }
    }
}