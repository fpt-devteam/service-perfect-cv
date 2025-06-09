using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{

    public class UpdateEducationRequest
    {
        [Required(ErrorMessage = "Education ID is required.")]
        public Guid Id { get; init; }

        [StringLength(100, ErrorMessage = "Degree cannot exceed 100 characters.")]
        public string Degree { get; init; } = null!;

        [StringLength(200, ErrorMessage = "Institution cannot exceed 100 characters.")]
        public string Institution { get; init; } = null!;

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string? Location { get; init; } = null!;

        public int? YearObtained { get; init; } = null!;

        [StringLength(100, ErrorMessage = "Minor cannot exceed 100 characters.")]
        public string? Minor { get; init; }

        [Range(0, 10.0, ErrorMessage = "GPA must be between 0 and 10.")]
        public decimal? Gpa { get; init; } = null!;

        [StringLength(500, ErrorMessage = "Additional information cannot exceed 500 characters.")]
        public string? AdditionalInfo { get; init; } = null!;
    }
}