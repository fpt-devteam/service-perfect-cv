using System;

namespace ServicePerfectCV.Application.DTOs.Experience.Responses
{
    public class ExperienceResponse
    {
        public required Guid Id { get; init; }
        public required Guid CVId { get; init; }
        public required string JobTitle { get; init; }
        public required Guid EmploymentTypeId { get; init; }
        public required string EmploymentTypeName { get; init; }
        public required string Organization { get; init; }
        public string? Location { get; init; }
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public string? Description { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
    }
}