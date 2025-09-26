using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed class ScoreCvJobHandler(ILlmClient llmClient) : IJobHandler
    {
        public JobType JobType => JobType.ScoreCV;

        public async Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken)
        {
            try
            {
                var inputJson = job.Input.RootElement.GetRawText();
                var completion = await llmClient.CompleteAsync(inputJson, cancellationToken);
                var output = JsonDocument.Parse(completion);
                return JobHandlerResult.Success(output);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return JobHandlerResult.Failure("score_cv.error", ex.Message);
            }
        }
    }
}
