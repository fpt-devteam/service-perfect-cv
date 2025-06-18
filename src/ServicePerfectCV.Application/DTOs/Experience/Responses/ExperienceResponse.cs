using System;

namespace ServicePerfectCV.Application.DTOs.Experience.Responses
{
    public class ExperienceResponse
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public Guid? JobTitleId { get; init; }
        public string? JobTitle { get; init; }
        public Guid EmploymentTypeId { get; init; }
        public string? EmploymentTypeName { get; init; }
        public Guid? CompanyId { get; init; }
        public string? Company { get; init; }
        public string? Location { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
