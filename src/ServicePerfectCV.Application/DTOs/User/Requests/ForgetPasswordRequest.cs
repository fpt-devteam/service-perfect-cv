using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.User.Requests
{
    public class ForgetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
} 