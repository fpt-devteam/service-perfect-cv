using System;
using System.Text.Json;
using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Domain.Entities
{
    public class Job : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public JobType Type { get; private set; }
        public JobStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? StartedAt { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }
        public JsonDocument Input { get; private set; } = null!;
        public JsonDocument? Output { get; private set; }
        public string? ErrorCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int Priority { get; private set; }

        public static Job Create(
            Guid id,
            JobType type,
            JsonDocument input,
            int priority,
            DateTimeOffset createdAt)
        {
            return new Job
            {
                Id = id,
                Type = type,
                Input = input,
                Priority = priority,
                CreatedAt = createdAt,
                Status = JobStatus.Queued
            };
        }

        public bool IsTerminal => Status is JobStatus.Succeeded or JobStatus.Failed or JobStatus.Canceled;

        public void MarkRunning(DateTimeOffset startedAt)
        {
            if (Status == JobStatus.Canceled)
                throw new InvalidOperationException("Cannot run a canceled job.");

            Status = JobStatus.Running;
            StartedAt = startedAt;
        }

        public void MarkSucceeded(JsonDocument output, DateTimeOffset completedAt)
        {
            if (Status != JobStatus.Running)
                throw new InvalidOperationException("Job must be running to succeed.");

            Output = output;
            Status = JobStatus.Succeeded;
            CompletedAt = completedAt;
            ErrorCode = null;
            ErrorMessage = null;
        }

        public void MarkFailed(string? errorCode, string? errorMessage, DateTimeOffset completedAt)
        {
            Status = JobStatus.Failed;
            CompletedAt = completedAt;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }


        public void MarkCanceled(DateTimeOffset completedAt)
        {
            if (IsTerminal)
                return;

            Status = JobStatus.Canceled;
            CompletedAt = completedAt;
        }

    }
}
