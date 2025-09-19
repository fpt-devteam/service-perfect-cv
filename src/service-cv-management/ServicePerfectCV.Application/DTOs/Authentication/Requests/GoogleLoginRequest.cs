using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }
    }
}