using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Helpers;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;


namespace ServicePerfectCV.Infrastructure;

public static class SemanticKernelInstaller
{
    public static IServiceCollection AddSemanticKernelInfra(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SemanticKernelOptions>(config.GetSection("SemanticKernel"));
        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;
            return CreateKernelByProvider(opts);
        });

        services.AddScoped<IAIOrchestrator, AIOrchestrator>();
        services.AddScoped<IJsonHelper, JsonHelper>();
        services.AddScoped<ISectionRubricBuilder, SectionRubricBuilder>();
        services.AddScoped<SectionScoreService>();

        return services;
    }

    private static Kernel CreateKernelByProvider(SemanticKernelOptions options)
    {
        return options.Provider switch
        {
            AIProvider.OpenAI => CreateOpenAIKernel(options),
            AIProvider.GoogleAI => CreateGoogleAIKernel(options),
            AIProvider.Ollama => CreateOllamaKernel(options),
            _ => throw new NotSupportedException($"AI Provider '{options.Provider}' is not supported.")
        };
    }

    private static Kernel CreateOpenAIKernel(SemanticKernelOptions options)
    {
        return Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: options.OpenAIModel,
                apiKey: options.OpenAIApiKey
            )
            .Build();
    }

    private static Kernel CreateGoogleAIKernel(SemanticKernelOptions options)
    {
        return Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(
                modelId: options.GoogleAIModel,
                apiKey: options.GoogleAIApiKey
            )
            .Build();
    }

    private static Kernel CreateOllamaKernel(SemanticKernelOptions options)
    {
        return Kernel.CreateBuilder()
            .AddOllamaChatCompletion(
                modelId: options.OllamaAIModel,
                endpoint: new Uri(options.OllamaEndpoint)
            )
       .Build();
    }
}