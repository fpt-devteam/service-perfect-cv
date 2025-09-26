using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed record QueuedJob(Guid JobId, JobType JobType, int Priority, DateTimeOffset VisibleAt);
}
