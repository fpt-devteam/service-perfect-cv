using ServicePerfectCV.Application.Interfaces.AI;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class PromptService : IPromptService
{
    private readonly Kernel _kernel;
    private readonly SemanticKernelOptions _opts;

    public PromptService(Kernel kernel, IOptions<SemanticKernelOptions> opts)
    {
        _kernel = kernel;
        _opts = opts.Value;
    }

    public async Task<string> CompleteAsync(string prompt, IDictionary<string, string>? variables = null, CancellationToken ct = default)
    {
        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory();
        history.AddSystemMessage("You are a helpful assistant. Reply in Vietnamese unless asked otherwise.");
        history.AddUserMessage(Interpolate(prompt, variables));
        var resp = await chat.GetChatMessageContentAsync(
            Interpolate(prompt, variables),
            new OpenAIPromptExecutionSettings
            {
                Temperature = _opts.Temperature ?? 0.2f,
                MaxTokens = _opts.MaxTokens ?? 1024
            },
            kernel: _kernel,
            ct);
        return resp?.Content ?? string.Empty;
    }

    private static string Interpolate(string src, IDictionary<string, string>? vars)
    {
        if (vars is null || vars.Count == 0) return src;
        foreach (var (k, v) in vars)
            src = src.Replace($"{{{{{k}}}}}", v);
        return src;
    }
}
