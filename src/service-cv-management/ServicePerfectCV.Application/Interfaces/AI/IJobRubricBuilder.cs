using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Infrastructure.Constants;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    public interface IJobRubricBuilder
    {
        Task<JobRubric> BuildJobRubricAsync(JobDescription jd, CancellationToken ct = default);
        Dictionary<string, List<string>> GetSystemRubricForLevel(string level);
        Dictionary<string, List<string>> GetFallbackRubric(JobDescription jd);
    }
}
