using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Extensions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class AIOrchestrator : IAIOrchestrator
{
    private readonly ISectionRubricBuilder _rubricBuilder;
    private readonly SectionScoreService _sectionScoreService;
    private readonly IJsonHelper _jsonHelper;

    private static readonly ILogger<AIOrchestrator> _logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger<AIOrchestrator>();

    public AIOrchestrator(
        ISectionRubricBuilder rubricBuilder,
        SectionScoreService sectionScoreService,
        IJsonHelper jsonHelper)
    {
        _rubricBuilder = rubricBuilder;
        _sectionScoreService = sectionScoreService;
        _jsonHelper = jsonHelper;
    }

    public async Task<SectionRubricDictionary> BuildCvSectionRubricsAsync(
        JobDescription jd,
        CancellationToken ct = default)
    {
        return await _rubricBuilder.BuildSectionRubricsAsync(jd, ct);
    }

    public async Task<CvAnalysisFinalOutput> ScoreCvSectionsAgainstRubricAsync(
        CvEntity cv,
        SectionRubricDictionary rubricDictionary,
        CancellationToken ct = default)
    {
        var sectionScores = await _sectionScoreService.ScoreAllSectionsAsync(
           rubricDictionary: rubricDictionary.ToSerializedDictionary(_jsonHelper),
           contentDictionary: cv.ToContentDictionary(_jsonHelper),
           ct: ct
        );

        _logger.LogDebug("Section Scores: {Scores}", _jsonHelper.Serialize(sectionScores));

        // 4. Calculate overall assessment
        //var overallAssessment = await CalculateOverallAssessmentAsync(jd, sectionScores, ct);

        return new CvAnalysisFinalOutput(
            sectionScores: sectionScores
        //overallAssessment: overallAssessment
        );
    }

    //    private Dictionary<string, string> PrepareCvSections(CvEntity cv)
    //    {
    //        return new Dictionary<string, string>
    //        {
    //            ["contact"] = SerializeSafely(cv.Contact),
    //            ["summary"] = SanitizeForPrompt(cv.CareerObjective),
    //            ["skills"] = SerializeSafely(cv.TechnicalSkills),
    //            ["experience"] = SerializeSafely(cv.Experience),
    //            ["projects"] = SerializeSafely(cv.Projects),
    //            ["education"] = SerializeSafely(cv.Education),
    //            ["certifications"] = SerializeSafely(cv.Achievements)
    //        };
    //    }

    //    private async Task<OverallDto> CalculateOverallAssessmentAsync(
    //        JobDescription jd,
    //        SectionFeedbackMap sectionScores,
    //        CancellationToken ct)
    //    {
    //        var sectionWeights = new Dictionary<string, double>
    //        {
    //            ["contact"] = 0.10,
    //            ["summary"] = 0.15,
    //            ["skills"] = 0.25,
    //            ["experience"] = 0.30,
    //            ["projects"] = 0.15,
    //            ["education"] = 0.10,
    //            ["certifications"] = 0.05
    //        };

    //        var weightedScores = new Dictionary<string, double>
    //        {
    //            ["contact"] = sectionScores.contact.weighted_score,
    //            ["summary"] = sectionScores.summary.weighted_score,
    //            ["skills"] = sectionScores.skills.weighted_score,
    //            ["experience"] = sectionScores.experience.weighted_score,
    //            ["projects"] = sectionScores.projects.weighted_score,
    //            ["education"] = sectionScores.education.weighted_score,
    //            ["certifications"] = sectionScores.certifications.weighted_score
    //        };

    //        var overallScore = weightedScores.Sum(kvp => kvp.Value * sectionWeights[kvp.Key]);
    //        var readinessScore = (int)Math.Round(overallScore * 20); // Convert 0-5 to 0-100
    //        var readinessLevel = DetermineReadinessLevel(readinessScore);
    //        var summaryText = await GenerateOverallSummaryAsync(jd, sectionScores, ct);

    //        return new OverallDto(readinessScore, readinessLevel, summaryText, weightedScores);
    //    }

    //    private string DetermineReadinessLevel(int score)
    //    {
    //        return score switch
    //        {
    //            >= 85 => "Excellent - Highly qualified candidate",
    //            >= 70 => "Good - Well-qualified candidate",
    //            >= 55 => "Average - Meets basic requirements",
    //            >= 40 => "Below Average - Some gaps identified",
    //            _ => "Poor - Significant improvements needed"
    //        };
    //    }

    //    private async Task<string> GenerateOverallSummaryAsync(
    //        JobDescription jd,
    //        SectionFeedbackMap sectionScores,
    //        CancellationToken ct)
    //    {
    //        var summarySettings = new OpenAIPromptExecutionSettings
    //        {
    //            Temperature = 0.3,
    //            MaxTokens = 500
    //        };

    //        var overallSummaryFn = _kernel.CreateFunctionFromPrompt(
    //            @"Generate a concise overall assessment summary for this CV analysis.

    //JOB POSITION: {{ $jobTitle }} ({{ $jobLevel }})

    //SECTION SCORES:
    //Contact: {{ $contactScore }}/5
    //Summary: {{ $summaryScore }}/5  
    //Skills: {{ $skillsScore }}/5
    //Experience: {{ $experienceScore }}/5
    //Projects: {{ $projectsScore }}/5
    //Education: {{ $educationScore }}/5
    //Certifications: {{ $certificationsScore }}/5

    //KEY FINDINGS:
    //{{ $keyFindings }}

    //Provide a 2-3 sentence executive summary highlighting:
    //1. Overall fit for the role
    //2. Top 2 strengths
    //3. Primary area for improvement",
    //            executionSettings: summarySettings
    //        );

    //        var keyFindings = ExtractKeyFindings(sectionScores);

    //        var result = await _kernel.InvokeAsync(overallSummaryFn, new KernelArguments
    //        {
    //            ["jobTitle"] = jd.Title,
    //            ["jobLevel"] = jd.Level,
    //            ["contactScore"] = sectionScores.contact.overall_score_0_5,
    //            ["summaryScore"] = sectionScores.summary.overall_score_0_5,
    //            ["skillsScore"] = sectionScores.skills.overall_score_0_5,
    //            ["experienceScore"] = sectionScores.experience.overall_score_0_5,
    //            ["projectsScore"] = sectionScores.projects.overall_score_0_5,
    //            ["educationScore"] = sectionScores.education.overall_score_0_5,
    //            ["certificationsScore"] = sectionScores.certifications.overall_score_0_5,
    //            ["keyFindings"] = keyFindings
    //        }, ct);

    //        return result.ToString();
    //    }

}