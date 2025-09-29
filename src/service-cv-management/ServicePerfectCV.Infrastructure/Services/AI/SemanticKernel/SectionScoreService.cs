using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Infrastructure.Constants;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public class SectionScoreService
    {
        private readonly ILogger<SectionScoreService> _logger;
        private readonly Kernel _kernel;
        private readonly SemanticKernelOptions _options;
        private readonly IJsonHelper _jsonHelper;

        public SectionScoreService(ILogger<SectionScoreService> logger, Kernel kernel, IOptions<SemanticKernelOptions> options, IJsonHelper jsonHelper)
        {
            _logger = logger;
            _kernel = kernel;
            _options = options.Value;
            _jsonHelper = jsonHelper;
        }
        public async Task<SectionScoreDictionary> ScoreAllSectionsAsync(
           Dictionary<SectionType, string> rubricDictionary,
           Dictionary<SectionType, string> contentDictionary,
           CancellationToken ct)
        {
            var scoringTasks = contentDictionary.Keys.Select(async sectionKey =>
            {
                var sectionRubric = rubricDictionary.TryGetValue(sectionKey, out string? value) ? value : string.Empty;

                var tmp = new KeyValuePair<SectionType, SectionScore>(
                    sectionKey,
                    await ScoreSectionAsync(sectionRubric, contentDictionary[sectionKey], sectionKey.ToString().ToLower(), ct)
                );
                return tmp;
            });

            var sectionResults = await Task.WhenAll(scoringTasks);
            return new SectionScoreDictionary(sectionResults);
        }

        public async Task<SectionScore> ScoreSectionAsync(
            string sectionRubric,
            string sectionContent,
            string sectionName,
            CancellationToken ct)
        {
            var scoreSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = _options.Temperature,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "SectionScore",
                    jsonSchema: BinaryData.FromString(data: _jsonHelper.GenerateJsonSchema<SectionScore>()),
                    jsonSchemaIsStrict: true
                ),
                MaxTokens = _options.MaxTokens
            };

            var sectionScoreFn = _kernel.CreateFunctionFromPrompt(
                promptTemplate: PromptManager.GetSectionScoringPrompt(),
                executionSettings: scoreSettings
            );

            try
            {
                var sectionScoreResult = await _kernel.InvokeAsync(sectionScoreFn, new KernelArguments
                {
                    ["sectionName"] = PromptSanitizeHelper.SanitizeInput(sectionName),
                    ["sectionRubric"] = PromptSanitizeHelper.SanitizeInput(sectionRubric),
                    ["sectionContent"] = PromptSanitizeHelper.SanitizeInput(sectionContent),
                    ["sectionScoreSchema"] = _jsonHelper.GenerateJsonSchema<SectionScore>()
                }, ct);

                var sectionScoreResultJson = sectionScoreResult.ToString();

                sectionScoreResultJson = CleanJsonResponse(sectionScoreResultJson);

                var sectionScore = _jsonHelper.Deserialize<SectionScore>(sectionScoreResultJson)
                    ?? new SectionScore();
                sectionScore.TotalScore0To5 = (int)sectionScore.CriteriaScores.Sum(c => c.Score0To5 * c.Weight0To1);
                return sectionScore;
            }
            catch (JsonException ex)
            {
                _logger.LogError("Failed to parse section score for {Section}: {Error}", sectionName, ex.Message);
                return new SectionScore();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error scoring section {Section}: {Error}", sectionName, ex.Message);
                return new SectionScore();
            }
        }

        private static string CleanJsonResponse(string? jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
                return string.Empty;

            var cleaned = jsonResponse.Trim();

            if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(7);
            }
            else if (cleaned.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(3);
            }

            if (cleaned.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 3);
            }

            return cleaned.Trim();
        }
    }
}