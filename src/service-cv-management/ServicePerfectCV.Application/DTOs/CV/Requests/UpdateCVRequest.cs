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
        public string Title { get; init; } = null!;
        public JobDetailDto? JobDetail { get; init; }
        public Guid? AnalysisId { get; init; }
    }
}