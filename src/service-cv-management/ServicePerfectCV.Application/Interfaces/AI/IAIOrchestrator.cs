using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.DTOs.JobDescription;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Application.Interfaces.AI;

public interface IAIOrchestrator
{
    Task<CvAnalysisFinalOutput> ScoreCvSectionsAgainstRubricAsync(CvEntity cv, SectionRubricDictionary rubricDictionary, CancellationToken ct = default);
    Task<SectionRubricDictionary> BuildCvSectionRubricsAsync(JobDescriptionRubricInputDto jd, CancellationToken ct = default);
}