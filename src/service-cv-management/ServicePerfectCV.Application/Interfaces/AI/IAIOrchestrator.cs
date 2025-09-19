using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;

namespace ServicePerfectCV.Application.Interfaces.AI;

public interface IAIOrchestrator
{
    Task<CvAnalysisFinalOutput> ScoreCvSectionsAgainstRubricAsync(CvEntity cv, SectionRubricDictionary rubricDictionary, CancellationToken ct = default);
    Task<SectionRubricDictionary> BuildCvSectionRubricsAsync(JobDescription jd, CancellationToken ct = default);
}