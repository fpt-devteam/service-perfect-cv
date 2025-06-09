using ServicePerfectCV.Domain.ValueObjects;
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
        public required string Title { get; init; }
        public JobDetail? JobDetail { get; init; } = null;
    }
}