using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class ListEducationRequest
    {
        [Required(ErrorMessage = "CVId is required")]
        public required Guid CVId { get; init; }
    }
}