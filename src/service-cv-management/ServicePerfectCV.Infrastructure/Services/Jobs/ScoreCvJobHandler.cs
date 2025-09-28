using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System.Text.Json;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed class ScoreCvJobHandler(IAIOrchestrator aiOrchestrator) : IJobHandler
    {
        public JobType JobType => JobType.ScoreCV;

        public async Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken)
        {
            try
            {
                // Parse input JSON to get CV and rubric data
                var inputElement = job.Input.RootElement;
                
                // Extract CV and SectionRubricDictionary from input
                var cvData = inputElement.GetProperty("cv");
                var rubricData = inputElement.GetProperty("rubric");
                
                // Deserialize CV and rubric
                var cv = JsonSerializer.Deserialize<CvEntity>(cvData.GetRawText());
                var rubric = JsonSerializer.Deserialize<SectionRubricDictionary>(rubricData.GetRawText());
                
                if (cv == null || rubric == null)
                {
                    return JobHandlerResult.Failure("score_cv.invalid_input", "Invalid CV or rubric data");
                }

                // Use AIOrchestrator to score CV
                var result = await aiOrchestrator.ScoreCvSectionsAgainstRubricAsync(cv, rubric, cancellationToken);
                
                // Serialize result to JSON
                var outputJson = JsonSerializer.Serialize(result);
                var output = JsonDocument.Parse(outputJson);
                
                return JobHandlerResult.Success(output);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                return JobHandlerResult.Failure("score_cv.json_error", $"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return JobHandlerResult.Failure("score_cv.error", ex.Message);
            }
        }
    }
}
