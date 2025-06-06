using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class ResendEmailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }
    }
}