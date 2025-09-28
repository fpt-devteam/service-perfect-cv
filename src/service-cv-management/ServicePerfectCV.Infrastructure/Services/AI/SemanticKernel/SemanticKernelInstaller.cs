using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Helpers;


namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public static class SemanticKernelInstaller
{

    public static IServiceCollection AddSemanticKernelInfra(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("ai", c =>
        {
            c.Timeout = TimeSpan.FromMinutes(5);
        });

        services.Configure<SemanticKernelOptions>(config.GetSection("SemanticKernel"));
        services.AddSingleton(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ai");
            var opts = sp.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;
            return CreateKernelByProvider(opts, http);
        });

        services.AddScoped<IAIOrchestrator, AIOrchestrator>();
        services.AddScoped<IJsonHelper, JsonHelper>();
        services.AddScoped<SectionRubricService>();
        services.AddScoped<SectionScoreService>();

        return services;
    }

    private static Kernel CreateKernelByProvider(SemanticKernelOptions options, HttpClient httpClient)
    {
        return options.Provider switch
        {
            AIProvider.OpenAI => CreateOpenAIKernel(options, httpClient),
            AIProvider.GoogleAI => CreateGoogleAIKernel(options, httpClient),
            AIProvider.Ollama => CreateOllamaKernel(options, httpClient),
            _ => throw new NotSupportedException($"AI Provider '{options.Provider}' is not supported.")
        };
    }

    private static Kernel CreateOpenAIKernel(SemanticKernelOptions options, HttpClient httpClient)
    {
        return Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: options.OpenAIModel,
                apiKey: options.OpenAIApiKey,
                httpClient: httpClient
            )
            .Build();
    }

    private static Kernel CreateGoogleAIKernel(SemanticKernelOptions options, HttpClient httpClient)
    {
        return Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(
                modelId: options.GoogleAIModel,
                apiKey: options.GoogleAIApiKey,
                httpClient: httpClient
            )
            .Build();
    }

    private static Kernel CreateOllamaKernel(SemanticKernelOptions options, HttpClient httpClient)
    {
        return Kernel.CreateBuilder()
            .AddOllamaChatCompletion(
                modelId: options.OllamaAIModel,
                endpoint: new Uri(options.OllamaEndpoint)
            )
            .Build();
    }
}