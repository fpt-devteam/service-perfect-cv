using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class OauthExchangeCodeRequest
    {
        [Required(ErrorMessage = "Code is required.")]
        public required string Code { get; init; }
        [Required(ErrorMessage = "Provider is required.")]
        public required string Provider { get; init; }
        public string? State { get; init; }
    }
}
