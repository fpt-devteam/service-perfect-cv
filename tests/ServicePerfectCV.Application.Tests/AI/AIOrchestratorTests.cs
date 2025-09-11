using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
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
        var orchestrator = new AIOrchestrator(mockPrompt.Object, plugin);

        var res = await orchestrator.ReviewCvAgainstJdAsync("cv", "jd");
        Assert.Equal("OK", res);
    }
}
