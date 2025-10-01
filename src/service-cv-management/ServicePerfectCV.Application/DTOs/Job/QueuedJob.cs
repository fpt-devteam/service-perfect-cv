using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Job
{
    public sealed record QueuedJob(Guid JobId, JobType JobType, int Priority, DateTimeOffset VisibleAt);
}
