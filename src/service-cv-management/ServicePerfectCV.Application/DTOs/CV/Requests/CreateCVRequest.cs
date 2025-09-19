using ServicePerfectCV.Application.DTOs.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Requests
{
    public class CreateCVRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public required string Title { get; init; }
        public required CreateJobDescriptionRequest JobDescription { get; init; }
    }

    public class CreateJobDescriptionRequest
    {
        public required string Title { get; init; }
        public required string CompanyName { get; init; }
        public required string Responsibility { get; init; }
        public required string Qualification { get; init; }
    }
}