using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = null;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 character long.")]
        public string NewPassword { get; set; } = null;

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password & confirmation password do not match! Please check your confirmation password.")]
        public string ConfirmPassword { get; set; } = null;
    }
}
