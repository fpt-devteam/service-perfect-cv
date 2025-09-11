using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ServicePerfectCV.Infrastructure;

public static class SemanticKernelInstaller
{
    public static IServiceCollection AddSemanticKernelInfra(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SemanticKernelOptions>(config.GetSection("SemanticKernel"));

        // Shared resilient HttpClient for LLM calls (timeout + simple retry)
        services.AddHttpClient("sk-ollama")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;
            var baseUrl = opts.OpenAIBaseUrl ?? "http://localhost:11434/v1";
            var apiKey = ResolveSecret(opts.OpenAIApiKey) ?? "ollama-dev"; // Ollama accepts any non-empty key
            var modelId = string.IsNullOrWhiteSpace(opts.OpenAIModel) ? "llama3" : opts.OpenAIModel!;

            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("sk-ollama");
            httpClient.BaseAddress = new Uri(baseUrl);

            var builder = Kernel.CreateBuilder();

            // Use OpenAI connector pointed to Ollama's OpenAI-compatible endpoint
            builder.AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: apiKey,
                httpClient: httpClient);

            return builder.Build();
        });

        services.AddScoped<IPromptService, PromptService>();
        services.AddScoped<IAIOrchestrator, AIOrchestrator>();
        services.AddScoped<CvReviewerPlugin>();

        return services;
    }

    private static string? ResolveSecret(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        if (value.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
        {
            var envKey = value.Substring(4);
            return Environment.GetEnvironmentVariable(envKey);
        }
        return value;
    }

}
