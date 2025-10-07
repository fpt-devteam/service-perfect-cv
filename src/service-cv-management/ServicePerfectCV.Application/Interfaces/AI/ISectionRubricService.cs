using ServicePerfectCV.Application.DTOs.JobDescription;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    public interface ISectionRubricService
    {
        Task<SectionRubricDictionary> BuildSectionRubricsAsync(JobDescriptionRubricInputDto jd, CancellationToken ct = default);
    }
}
