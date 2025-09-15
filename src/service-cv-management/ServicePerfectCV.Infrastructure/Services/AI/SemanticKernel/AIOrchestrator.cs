using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class AIOrchestrator : IAIOrchestrator
{
    private readonly IPromptService _prompter;
    private readonly CvReviewerPlugin _plugin;
    private readonly Kernel _kernel;
    private readonly IJobRubricBuilder _rubricBuilder;

    private static readonly ILogger<AIOrchestrator> _logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger<AIOrchestrator>();

    public AIOrchestrator(IPromptService prompter, CvReviewerPlugin plugin, Kernel kernel, IJobRubricBuilder rubricBuilder)
    {
        _prompter = prompter;
        _plugin = plugin;
        _kernel = kernel;
        _rubricBuilder = rubricBuilder;
    }
    public async Task<CvAnalysisFinalOutput> AnalyzeCvWithSemanticKernelAsync(
        CvEntity cv,
        JobDescription jd,
        CancellationToken ct = default
    )
    {
        // 1. Build rubric based on job description
        //var rubric = await BuildJobRubricAsync(jd, ct);
        _logger.LogInformation("Building rubric for Job: {JobTitle}", jd.Title);
        var rubric = await _rubricBuilder.BuildJobRubricAsync(jd, ct);
        _logger.LogInformation("Generated Rubric: {Rubric}", JsonSerializer.Serialize(rubric));

        //// 2. Extract and prepare CV sections
        //var sectionTexts = PrepareCvSections(cv);

        //// 3. Score each section against the rubric
        //var sectionScores = await ScoreAllSectionsAsync(rubric, sectionTexts, ct);

        //// 4. Calculate overall assessment
        //var overallAssessment = await CalculateOverallAssessmentAsync(jd, sectionScores, ct);

        return new CvAnalysisFinalOutput();
    }

    #region Private Helper Methods

    private async Task<Dictionary<string, List<string>>> BuildJobRubricAsync(
        JobDescription jd,
        CancellationToken ct)
    {
        var rubricSchema =
        """
        {
            "type":"object",
            "properties":{
                "contact":{"type":"array","items":{"type":"string"},"maxItems":5},
                "summary":{"type":"array","items":{"type":"string"},"maxItems":5},
                "skills":{"type":"array","items":{"type":"string"},"maxItems":8},
                "experience":{"type":"array","items":{"type":"string"},"maxItems":8},
                "projects":{"type":"array","items":{"type":"string"},"maxItems":6},
                "education":{"type":"array","items":{"type":"string"},"maxItems":4},
                "certifications":{"type":"array","items":{"type":"string"},"maxItems":4}
            },
            "required":["contact","summary","skills","experience","projects","education","certifications"],
            "additionalProperties":false
        }
        """;

        var rubricSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.1,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "CvRubric",
                jsonSchema: BinaryData.FromString(rubricSchema),
                jsonSchemaIsStrict: true
            ),
            MaxTokens = 2048
        };

        var rubricBuilderFn = _kernel.CreateFunctionFromPrompt(
            """
            You create concise rubrics per CV section for a given job description. 
            Return JSON only with keys: contact, summary, skills, experience, projects, education, certifications. 
            Each key has an array of specific, measurable criteria relevant to this job.
            If any section has key word Volunteer, that section will be scored 0.
            
            Job Title: {{ $title }}
            Job Level: {{ $level }}
            Key Requirements:
            {{ $requirements }}
            
            Return valid JSON only.
            """,
            executionSettings: rubricSettings
        );

        var rubricResult = await _kernel.InvokeAsync(rubricBuilderFn, new KernelArguments
        {
            ["title"] = SanitizeForPrompt(jd.Title),
            ["level"] = SanitizeForPrompt(jd.Level),
            ["requirements"] = PrepareJobRequirements(jd)
        }, ct);

        var rubricJson = rubricResult.ToString();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(rubricJson)
                   ?? new Dictionary<string, List<string>>();
        }
        catch (JsonException ex)
        {
            // Log error and return empty rubric
            _logger.LogWarning("Failed to parse rubric JSON: {Error}", ex.Message);
            return new Dictionary<string, List<string>>();
        }
    }

    private Dictionary<string, string> PrepareCvSections(CvEntity cv)
    {
        return new Dictionary<string, string>
        {
            ["contact"] = SerializeSafely(cv.Contact),
            ["summary"] = SanitizeForPrompt(cv.CareerObjective),
            ["skills"] = SerializeSafely(cv.TechnicalSkills),
            ["experience"] = SerializeSafely(cv.Experience),
            ["projects"] = SerializeSafely(cv.Projects),
            ["education"] = SerializeSafely(cv.Education),
            ["certifications"] = SerializeSafely(cv.Achievements)
        };
    }

    private async Task<SectionFeedbackMap> ScoreAllSectionsAsync(
        Dictionary<string, List<string>> rubric,
        Dictionary<string, string> sectionTexts,
        CancellationToken ct)
    {
        var scoringTasks = sectionTexts.Keys.Select(async sectionKey =>
            new KeyValuePair<string, SectionScoreDto>(
                sectionKey,
                await ScoreSectionAsync(rubric, sectionTexts, sectionKey, ct)
            )
        );

        var sectionResults = await Task.WhenAll(scoringTasks);
        var resultDict = sectionResults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return new SectionFeedbackMap(
            contact: resultDict["contact"],
            summary: resultDict["summary"],
            skills: resultDict["skills"],
            experience: resultDict["experience"],
            projects: resultDict["projects"],
            education: resultDict["education"],
            certifications: resultDict["certifications"]
        );
    }

    private async Task<SectionScoreDto> ScoreSectionAsync(
        Dictionary<string, List<string>> rubric,
        Dictionary<string, string> sectionTexts,
        string sectionKey,
        CancellationToken ct)
    {
        var scoreSchema = """
        {
            "type": "object",
            "properties": {
                "score_0_5": {"type": "integer", "minimum": 0, "maximum": 5},
                "strengths": {
                "type": "array", 
                "items": {"type": "string", "minLength": 5, "maxLength": 200},
                "maxItems": 5
                },
                "current_limitations": {
                "type": "array", 
                "items": {"type": "string", "minLength": 5, "maxLength": 200},
                "maxItems": 5
                },
                "improvement_suggestions": {
                "type": "array", 
                "items": {"type": "string", "minLength": 5, "maxLength": 200},
                "maxItems": 5
                }
            },
            "required": ["score_0_5", "strengths", "current_limitations", "improvement_suggestions"],
            "additionalProperties": false
        }
        """;

        var scoreSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.2,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "SectionScore",
                jsonSchema: BinaryData.FromString(scoreSchema),
                jsonSchemaIsStrict: false
            ),
            MaxTokens = 2028
        };

        var sectionScoreFn = _kernel.CreateFunctionFromPrompt(
            """
            You are a CV section scorer. Score the section content against the given criteria on a 0-5 integer scale.
            Provide specific, actionable feedback.
            
            Scoring Criteria:
            {{ $criteria }}
            
            Section Content:
            {{ $content }}
            
            Return valid JSON only with this exact schema:
            {
                "score_0_5": 0,
                "strengths": ["specific strength 1", "specific strength 2"],
                "current_limitations": ["specific limitation 1", "specific limitation 2"],
                "improvement_suggestions": ["actionable suggestion 1", "actionable suggestion 2"]
            }
            """,
            executionSettings: scoreSettings
        );

        var criteria = rubric.ContainsKey(sectionKey) && rubric[sectionKey].Any()
            ? string.Join("\n• ", rubric[sectionKey])
            : "General quality and completeness";

        _logger.LogWarning("Scoring Section: {Section}, Criteria: {Criteria}, Content: {Content}",
            sectionKey, criteria, sectionTexts[sectionKey]);

        var result = await _kernel.InvokeAsync(sectionScoreFn, new KernelArguments
        {
            ["criteria"] = criteria,
            ["content"] = sectionTexts[sectionKey]
        }, ct);

        try
        {
            var dto = JsonSerializer.Deserialize<SectionScoreDto>(
                result.ToString(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return dto ?? new SectionScoreDto(0, new(), new(), new());
        }
        catch (JsonException ex)
        {
            // Log error and return default score
            _logger.LogWarning("Failed to parse section score for {Section}: {Error}", sectionKey, ex.Message);
            return new SectionScoreDto(0,
                new() { "Unable to analyze this section" },
                new() { "Analysis failed" },
                new() { "Please review this section manually" });
        }
    }

    private async Task<OverallDto> CalculateOverallAssessmentAsync(
        JobDescription jd,
        SectionFeedbackMap sectionScores,
        CancellationToken ct)
    {
        var overallScore = CalculateWeightedScore(sectionScores);
        var readinessLevel = DetermineReadinessLevel(overallScore);
        var summaryText = await GenerateOverallSummaryAsync(jd, sectionScores, ct);

        return new OverallDto(overallScore, readinessLevel, summaryText);
    }

    private async Task<string> GenerateOverallSummaryAsync(
    JobDescription jd,
    SectionFeedbackMap sectionScores,
    CancellationToken ct)
    {
        var summarySettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.3,
            MaxTokens = 200
        };

        var overallSummaryFn = _kernel.CreateFunctionFromPrompt(
            promptTemplate: """
        Generate a concise 2-sentence summary of the candidate's readiness for this job.
        Focus on the strongest qualifications and one key area for improvement.
        
        Job: {{ $jobTitle }} ({{ $jobLevel }})
        Key Requirements: {{ $requirements }}
        
        Section Analysis Summary:
        Skills Score: {{ $skillsScore }}/5
        Experience Score: {{ $experienceScore }}/5  
        Projects Score: {{ $projectsScore }}/5
        Education Score: {{ $educationScore }}/5
        
        Return plain text only (maximum 2 sentences).
        """,
            executionSettings: summarySettings
        );

        try
        {
            var result = await _kernel.InvokeAsync(overallSummaryFn, new KernelArguments
            {
                ["jobTitle"] = SanitizeForPrompt(jd.Title),
                ["jobLevel"] = SanitizeForPrompt(jd.Level),
                ["requirements"] = PrepareJobRequirements(jd),
                ["skillsScore"] = sectionScores.skills.score_0_5,
                ["experienceScore"] = sectionScores.experience.score_0_5,
                ["projectsScore"] = sectionScores.projects.score_0_5,
                ["educationScore"] = sectionScores.education.score_0_5
            }, ct);

            var summary = result.ToString().Trim();
            return string.IsNullOrEmpty(summary) ? GetFallbackSummary(jd.Title) : summary;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to generate overall summary: {Error}", ex.Message);
            return GetFallbackSummary(jd.Title);
        }
    }


    private int CalculateWeightedScore(SectionFeedbackMap sectionScores)
    {
        var weights = new Dictionary<string, double>
        {
            ["contact"] = 0.05,
            ["summary"] = 0.10,
            ["skills"] = 0.30,
            ["experience"] = 0.25,
            ["projects"] = 0.20,
            ["education"] = 0.08,
            ["certifications"] = 0.02
        };

        double ConvertToPercentage(int score) => Math.Max(0, Math.Min(5, score)) * 20.0;

        var weightedTotal =
            weights["contact"] * ConvertToPercentage(sectionScores.contact.score_0_5) +
            weights["summary"] * ConvertToPercentage(sectionScores.summary.score_0_5) +
            weights["skills"] * ConvertToPercentage(sectionScores.skills.score_0_5) +
            weights["experience"] * ConvertToPercentage(sectionScores.experience.score_0_5) +
            weights["projects"] * ConvertToPercentage(sectionScores.projects.score_0_5) +
            weights["education"] * ConvertToPercentage(sectionScores.education.score_0_5) +
            weights["certifications"] * ConvertToPercentage(sectionScores.certifications.score_0_5);

        return (int)Math.Round(weightedTotal);
    }

    private static string DetermineReadinessLevel(int score)
    {
        return score switch
        {
            >= 70 => "Ready",
            >= 55 => "Nearly there",
            _ => "Needs work"
        };
    }

    private string PrepareJobRequirements(JobDescription jd)
    {
        if (jd.Requirements == null || !jd.Requirements.Any())
            return "No specific requirements listed";

        return string.Join("\n• ", jd.Requirements.Select(SanitizeForPrompt));
    }

    private string SerializeSafely(object? obj)
    {
        if (obj == null) return "";

        try
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            // Truncate if too long to avoid prompt limits
            return json.Length > 2000 ? json[..1997] + "..." : json;
        }
        catch
        {
            return obj.ToString() ?? "";
        }
    }

    private static string SanitizeForPrompt(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        // Remove or escape problematic characters for LLM prompts
        return input
            .Replace("{{", "{ {")
            .Replace("}}", "} }")
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Trim();
    }

    private string GetFallbackSummary(string jobTitle)
    {
        return $"Candidate shows potential for {jobTitle} position. Detailed manual review recommended for accurate assessment.";
    }
    #endregion


}
