using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.User.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public required string Code { get; set; }

        [Required]
        [MinLength(8)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
} 