namespace ServicePerfectCV.Application.Interfaces.AI;

public interface IAIOrchestrator
{
    Task<string> ReviewCvAgainstJdAsync(string cvText, string jdText, CancellationToken ct = default);
}
