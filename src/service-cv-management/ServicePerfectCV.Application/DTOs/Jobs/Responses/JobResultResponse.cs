using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Application.DTOs.Jobs.Responses
{
    public sealed class JobResultResponse
    {
        public Guid Id { get; init; }

        public required string Type { get; init; }

        public JobStatus Status { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? StartedAt { get; init; }

        public DateTimeOffset? CompletedAt { get; init; }

        public JsonElement InputJson { get; init; }

        public JsonElement? OutputJson { get; init; }

        public string? ErrorCode { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
