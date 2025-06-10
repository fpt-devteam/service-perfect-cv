using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class CreateEducationRequest
    {
        [Required(ErrorMessage = "CVId is required")]
        public required Guid CVId { get; init; }

        [Required(ErrorMessage = "Degree is required")]
        [MaxLength(100, ErrorMessage = "Degree cannot exceed 100 characters")]
        public required string Degree { get; init; }

        [Required(ErrorMessage = "Institution is required")]
        [MaxLength(200, ErrorMessage = "Institution cannot exceed 200 characters")]
        public required string Institution { get; init; }
        
        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; init; }

        public int? YearObtained { get; init; }

        public string? Minor { get; init; }

        public decimal? Gpa { get; init; }

        public string? AdditionalInfo { get; init; }
    }
}