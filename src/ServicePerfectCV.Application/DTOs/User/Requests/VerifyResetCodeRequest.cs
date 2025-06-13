using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.User.Requests
{
    public class VerifyResetCodeRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public required string Code { get; set; }
    }
} 