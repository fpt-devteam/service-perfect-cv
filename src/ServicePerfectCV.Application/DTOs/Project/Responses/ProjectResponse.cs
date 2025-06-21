using System;

namespace ServicePerfectCV.Application.DTOs.Project.Responses
{
    public class ProjectResponse
    {
        public required Guid Id { get; init; }
        public required Guid CVId { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public string? Link { get; init; }
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
