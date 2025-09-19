using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.Authentication.Responses
{
    public class GoogleUserInfo
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("given_name")]
        public string? GivenName { get; init; }

        [JsonPropertyName("family_name")]
        public string? FamilyName { get; init; }

        [JsonPropertyName("picture")]
        public string? Picture { get; init; }

        [JsonPropertyName("locale")]
        public string? Locale { get; init; }
    }
}