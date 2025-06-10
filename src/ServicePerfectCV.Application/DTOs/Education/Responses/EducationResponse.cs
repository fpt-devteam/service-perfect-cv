using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Education.Responses
{
    public class EducationResponse
    {
        public required string Institution { get; init; }
        public required string Degree { get; init; }
        public string? Location { get; init; }
        public int? YearObtained { get; init; }
        public string? Minor { get; init; }
        public decimal? Gpa { get; init; }
        public string? AdditionalInfo { get; init; }
    }
}