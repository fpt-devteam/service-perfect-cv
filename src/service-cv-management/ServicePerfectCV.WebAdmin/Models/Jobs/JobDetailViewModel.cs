using System.Text.Json;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.WebAdmin.Models.Jobs
{
    public class JobDetailViewModel
    {
        public Guid Id { get; set; }
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public int Priority { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string InputJson { get; set; } = string.Empty;
        public string? OutputJson { get; set; }
        public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue
            ? CompletedAt.Value - StartedAt.Value
            : null;
    }
}

