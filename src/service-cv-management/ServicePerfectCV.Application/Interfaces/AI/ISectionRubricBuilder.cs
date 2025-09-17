using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    public interface ISectionRubricBuilder
    {
        Task<SectionRubricDictionary> BuildSectionRubricsAsync(JobDescription jd, CancellationToken ct = default);
    }
}
