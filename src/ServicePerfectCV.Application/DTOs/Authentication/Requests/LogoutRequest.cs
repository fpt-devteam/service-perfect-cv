using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class LogoutRequest
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public required string RefreshToken { get; set; }
    }
}