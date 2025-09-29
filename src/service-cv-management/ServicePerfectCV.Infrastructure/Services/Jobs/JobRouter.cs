using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class JobRouter
    {
        private readonly Dictionary<JobType, IJobHandler> _handlers;

        public JobRouter(IEnumerable<IJobHandler> handlers)
        {
            _handlers = handlers.ToDictionary(handler => handler.JobType);
        }

        public IJobHandler Resolve(JobType jobType)
        {
            if (_handlers.TryGetValue(jobType, out var handler))
                return handler;

            throw new InvalidOperationException($"No job handler registered for type '{jobType}'.");
        }
    }
}
