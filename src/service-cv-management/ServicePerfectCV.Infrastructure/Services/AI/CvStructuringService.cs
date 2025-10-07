using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using ServicePerfectCV.Application.DTOs.CvStructuring;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Constants;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ServicePerfectCV.Infrastructure.Services.AI
{
    public sealed class CvStructuringService : ICvStructuringService
    {
        private readonly Kernel _kernel;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<CvStructuringService> _logger;

        public CvStructuringService(Kernel kernel, IJsonHelper jsonHelper, ILogger<CvStructuringService> logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _jsonHelper = jsonHelper ?? throw new ArgumentNullException(nameof(jsonHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CvSectionExtractionResult> StructureCvContentAsync(string rawText, CancellationToken cancellationToken = default)
        {
            var result = new CvSectionExtractionResult();

            if (string.IsNullOrWhiteSpace(rawText))
            {
                result.Errors.Add("Raw text cannot be empty");
                return result;
            }

            try
            {
                _logger.LogInformation("Starting CV content structuring. Text length: {Length}", rawText.Length);

                var promptTemplate = PromptManager.GetPrompt(PromptType.CvSectionExtraction);
                var arguments = new KernelArguments
                {
                    ["rawText"] = rawText
                };

                var response = await _kernel.InvokePromptAsync(promptTemplate, arguments, cancellationToken: cancellationToken);
                var responseText = response.ToString();

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    result.Errors.Add("LLM returned empty response");
                    return result;
                }

                _logger.LogDebug("LLM response received. Length: {Length}", responseText.Length);

                // Clean response - remove markdown code blocks if present
                responseText = CleanJsonResponse(responseText);

                // Parse the JSON response using IJsonHelper
                var extractedSections = _jsonHelper.Deserialize<ExtractedCVSectionsDto>(responseText);

                if (extractedSections == null)
                {
                    result.Errors.Add("Failed to deserialize LLM response");
                    return result;
                }

                // Convert each section to JsonDocument and map to SectionType
                if (extractedSections.Contact != null)
                {
                    result.Sections[SectionType.Contact] = _jsonHelper.SerializeToDocument(extractedSections.Contact);
                }

                if (extractedSections.Summary != null)
                {
                    result.Sections[SectionType.Summary] = _jsonHelper.SerializeToDocument(extractedSections.Summary);
                }

                if (extractedSections.Education != null && extractedSections.Education.Count > 0)
                {
                    result.Sections[SectionType.Education] = _jsonHelper.SerializeToDocument(extractedSections.Education);
                }

                if (extractedSections.Experience != null && extractedSections.Experience.Count > 0)
                {
                    result.Sections[SectionType.Experience] = _jsonHelper.SerializeToDocument(extractedSections.Experience);
                }

                if (extractedSections.Skills != null && extractedSections.Skills.Count > 0)
                {
                    result.Sections[SectionType.Skills] = _jsonHelper.SerializeToDocument(extractedSections.Skills);
                }

                if (extractedSections.Projects != null && extractedSections.Projects.Count > 0)
                {
                    result.Sections[SectionType.Projects] = _jsonHelper.SerializeToDocument(extractedSections.Projects);
                }

                if (extractedSections.Certifications != null && extractedSections.Certifications.Count > 0)
                {
                    result.Sections[SectionType.Certifications] = _jsonHelper.SerializeToDocument(extractedSections.Certifications);
                }

                _logger.LogInformation("CV content structuring completed successfully. Extracted {Count} sections", result.Sections.Count);

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error during CV structuring");
                result.Errors.Add($"JSON parsing error: {ex.Message}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CV content structuring");
                result.Errors.Add($"Structuring error: {ex.Message}");
                return result;
            }
        }

        private static string CleanJsonResponse(string response)
        {
            // Remove markdown code blocks
            response = Regex.Replace(response, @"```json\s*", "", RegexOptions.IgnoreCase);
            response = Regex.Replace(response, @"```\s*$", "", RegexOptions.IgnoreCase);
            response = response.Trim();
            return response;
        }
    }
}
