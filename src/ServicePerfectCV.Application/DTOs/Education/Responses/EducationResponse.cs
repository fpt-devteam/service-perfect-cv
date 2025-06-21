using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Education.Responses
{
    public class EducationResponse
    {
        public required string Organization { get; init; }
        public required string Degree { get; init; }
        public string? FieldOfStudy { get; init; }
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public string? Description { get; init; }
        public decimal? Gpa { get; init; }
    }
}