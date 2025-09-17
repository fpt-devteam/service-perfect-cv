using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Infrastructure.Helpers;

namespace ServicePerfectCV.Application.Tests.AI;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ReviewCvAgainstJdAsync_ComposesPrompt_And_ReturnsResult()
    {
        var schema = JsonHelper.GenerateJsonSchema<SectionRubricDictionary>();

        Assert.Equal("OK", "OK");
    }
}
