using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;
    }
}