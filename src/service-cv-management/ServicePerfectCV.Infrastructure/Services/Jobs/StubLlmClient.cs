using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ServicePerfectCV.Application.Interfaces.AI;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class StubLlmClient : ILlmClient
    {
        public async Task<string> CompleteAsync(string inputJson, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);

            using var parsed = JsonDocument.Parse(inputJson);
            var inputClone = parsed.RootElement.Clone();

            var payload = new
            {
                generatedAt = DateTimeOffset.UtcNow,
                score = 0.85,
                evaluation = "stubbed",
                input = inputClone
            };

            return JsonSerializer.Serialize(payload);
        }
    }
}
