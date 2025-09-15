using ServicePerfectCV.Application.DTOs.AI;

namespace ServicePerfectCV.Application.Interfaces.AI;

public interface IAIOrchestrator
{
    Task<CvAnalysisFinalOutput> AnalyzeCvWithSemanticKernelAsync(CvEntity cv, JobDescription jd, CancellationToken ct = default);
}
