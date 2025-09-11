namespace ServicePerfectCV.Application.Interfaces.AI;

public interface IPromptService
{
    Task<string> CompleteAsync(string prompt, IDictionary<string, string>? variables = null, CancellationToken ct = default);
}
