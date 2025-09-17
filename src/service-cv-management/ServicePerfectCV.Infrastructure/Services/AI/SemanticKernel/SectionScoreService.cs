using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public class SectionScoreService
    {
        private readonly ILogger<SectionScoreService> _logger;
        private readonly Kernel _kernel;
        private readonly SemanticKernelOptions _options;

        public SectionScoreService(ILogger<SectionScoreService> logger, Kernel kernel, IOptions<SemanticKernelOptions> options)
        {
            _logger = logger;
            _kernel = kernel;
            _options = options.Value;
        }
        public async Task<SectionScoreDictionary> ScoreAllSectionsAsync(
           Dictionary<Section, string> rubricDictionary,
           Dictionary<Section, string> contentDictionary,
           CancellationToken ct)
        {
            var scoringTasks = contentDictionary.Keys.Select(async sectionKey =>
            {
                var sectionRubric = rubricDictionary.TryGetValue(sectionKey, out string? value) ? value : string.Empty;

                return new KeyValuePair<Section, SectionScore>(
                    sectionKey,
                    await ScoreSectionAsync(sectionRubric, contentDictionary[sectionKey], sectionKey.ToString().ToLower(), ct)
                );
            });

            var sectionResults = await Task.WhenAll(scoringTasks);
            var resultDict = sectionResults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            _logger.LogDebug("Score all sections: {Scores}", JsonHelper.Serialize(resultDict));
            return new SectionScoreDictionary(resultDict);
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
                    jsonSchema: BinaryData.FromString(data: JsonHelper.GenerateJsonSchema<SectionScore>()),
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
                    ["sectionScoreSchema"] = JsonHelper.GenerateJsonSchema<SectionScore>()
                }, ct);

                var sectionScoreResultJson = sectionScoreResult.ToString();
                _logger.LogDebug("Section Score JSON for {Section}: {ScoreJson}", sectionName, sectionScoreResultJson);
                var sectionScore = JsonHelper.Deserialize<SectionScore>(sectionScoreResultJson ?? string.Empty)
                    ?? new SectionScore();
                sectionScore.TotalScore0To5 = sectionScore.CriteriaScores.Sum(c => (int)(c.Score0To5 * c.Weight0To1));
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
    }
}

