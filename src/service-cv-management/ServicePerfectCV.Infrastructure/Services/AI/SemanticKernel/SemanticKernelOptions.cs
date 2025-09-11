namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class SemanticKernelOptions
{
    // OpenAI-compatible (Ollama) settings
    public string? OpenAIModel { get; set; }           // e.g., "llama3.1:8b"
    public string? OpenAIApiKey { get; set; }          // e.g., "env:OLLAMA_API_KEY"
    public string? OpenAIBaseUrl { get; set; }         // e.g., "http://localhost:11434/v1"

    public int? MaxTokens { get; set; } = 1024;
    public float? Temperature { get; set; } = 0.2f;
}
