using ServicePerfectCV.Application.DTOs.JobDescription;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System.Text.Json;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed class BuildCvSectionRubricHandler(
        IAIOrchestrator aiOrchestrator,
        IJsonHelper jsonHelper,
        JobDescriptionService jobDescriptionService
        ) : IJobHandler
    {
        public JobType JobType => JobType.BuildCvSectionRubric;

        public async Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken)
        {
            SectionRubricDictionary? result = null;

            try
            {
                JobDescriptionRubricInputDto? jobDescriptionDto = jsonHelper.DeserializeFromElement<JobDescriptionRubricInputDto>(job.Input.RootElement);

                if (jobDescriptionDto == null)
                {
                    return JobHandlerResult.Failure("build_rubric.invalid_input", "Invalid job description data");
                }

                result = await aiOrchestrator.BuildCvSectionRubricsAsync(jobDescriptionDto, cancellationToken);

                await jobDescriptionService.UpdateSectionRubricAsync(jobDescriptionDto.JobDescriptionId, result);

                return JobHandlerResult.Success(jsonHelper.SerializeToDocument(result));
            }
            catch (OperationCanceledException ex)
            {
                var output = result != null ? jsonHelper.SerializeToDocument(result) : null;
                return JobHandlerResult.Failure("build_rubric.canceled", $"AI prompt execution was canceled: {ex.Message}", output);
            }
            catch (JsonException ex)
            {
                var output = result != null ? jsonHelper.SerializeToDocument(result) : null;
                return JobHandlerResult.Failure("build_rubric.json_error", $"JSON parsing error: {ex.Message}", output);
            }
            catch (Exception ex)
            {
                var output = result != null ? jsonHelper.SerializeToDocument(result) : null;
                return JobHandlerResult.Failure("build_rubric.error", ex.Message, output);
            }
        }
    }
}
