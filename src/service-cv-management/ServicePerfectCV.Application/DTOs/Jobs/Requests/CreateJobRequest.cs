using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Application.DTOs.Jobs.Requests
{
    public sealed class CreateJobRequest
    {
        public required JobType JobType { get; init; }

        public JsonElement InputJson { get; init; }

        public int Priority { get; init; }
    }
}
