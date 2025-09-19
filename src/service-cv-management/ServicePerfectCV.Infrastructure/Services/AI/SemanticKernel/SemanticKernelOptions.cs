namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public enum AIProvider
{
    OpenAI,
    GoogleAI,
    Ollama
}

public sealed class SemanticKernelOptions
{
    public AIProvider Provider { get; set; } = AIProvider.Ollama;

    public string OpenAIModel { get; set; } = "gpt-4o";
    public string OpenAIApiKey { get; set; } = string.Empty;

    public string GoogleAIModel { get; set; } = "gemini-1.5-flash";
    public string GoogleAIApiKey { get; set; } = string.Empty;

    public string OllamaAIModel { get; set; } = "llama3.1:latest";
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";

    public required int MaxTokens { get; set; }
    public required float Temperature { get; set; }
}