using ServicePerfectCV.Application.DTOs.CV;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Requests
{
    public class UpdateCVRequest
    {
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; init; } = null!;
        public UpdateJobDescriptionRequest? JobDescription { get; init; }
    }

    public class UpdateJobDescriptionRequest
    {
        public string? Title { get; init; }
        public string? CompanyName { get; init; }
        public string? Responsibility { get; init; }
        public string? Qualification { get; init; }
    }
}