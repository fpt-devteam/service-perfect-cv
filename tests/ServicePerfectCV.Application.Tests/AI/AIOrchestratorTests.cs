using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
using Microsoft.SemanticKernel;
using Moq;
using Xunit;

namespace ServicePerfectCV.Application.Tests.AI;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ReviewCvAgainstJdAsync_ComposesPrompt_And_ReturnsResult()
    {
        var mockPrompt = new Mock<IPromptService>();
        mockPrompt.Setup(p => p.CompleteAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                  .ReturnsAsync("OK");

        var plugin = new CvReviewerPlugin();

        // Create a mock Kernel for the constructor
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4", "test-key")
            .Build();

        var orchestrator = new AIOrchestrator(mockPrompt.Object, plugin, kernel);

        var res = await orchestrator.ReviewCvAgainstJdAsync("cv", "jd");
        Assert.Equal("OK", res);
    }
}
