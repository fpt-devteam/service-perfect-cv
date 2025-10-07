using System;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Package.Requests
{
    public class CreatePackageRequest
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Package name cannot exceed 100 characters")]
        public string Name { get; set; } = default!;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of credits must be at least 1")]
        public int NumCredits { get; set; }

        public bool IsActive { get; set; } = true;
    }
}