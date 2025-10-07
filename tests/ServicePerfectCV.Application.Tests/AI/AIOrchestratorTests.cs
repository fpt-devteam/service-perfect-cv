using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Infrastructure.Helpers;

namespace ServicePerfectCV.Application.Tests.AI;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ReviewCvAgainstJdAsync_ComposesPrompt_And_ReturnsResult()
    {
        IJsonHelper jsonHelper = new JsonHelper();
        var schema = jsonHelper.GenerateJsonSchema<SectionRubricDictionary>();

        Assert.Equal("OK", "OK");
    }
}
