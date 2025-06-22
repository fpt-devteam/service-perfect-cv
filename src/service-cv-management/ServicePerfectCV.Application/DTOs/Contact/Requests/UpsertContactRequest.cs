using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ServicePerfectCV.Application.Common;

namespace ServicePerfectCV.Application.DTOs.Contact.Requests
{
    public class UpsertContactRequest
    {
        [Required(ErrorMessage = "CV ID is required.")]
        public required Guid CVId { get; init; }

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be between 10-20 characters")]
        public string? PhoneNumber { get; set; }

        [JsonIgnore]
        public Optional<string?> Phone => PhoneNumber != null ? Optional<string?>.From(PhoneNumber) : Optional<string?>.None();

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        [JsonIgnore]
        public Optional<string?> EmailAddress => Email != null ? Optional<string?>.From(Email) : Optional<string?>.None();

        [Url(ErrorMessage = "Invalid LinkedIn URL format.")]
        public string? LinkedInUrl { get; set; }

        [JsonIgnore]
        public Optional<string?> LinkedIn => LinkedInUrl != null ? Optional<string?>.From(LinkedInUrl) : Optional<string?>.None();

        [Url(ErrorMessage = "Invalid GitHub URL format.")]
        public string? GitHubUrl { get; set; }

        [JsonIgnore]
        public Optional<string?> GitHub => GitHubUrl != null ? Optional<string?>.From(GitHubUrl) : Optional<string?>.None();

        [Url(ErrorMessage = "Invalid website URL format.")]
        public string? PersonalWebsiteUrl { get; set; }

        [JsonIgnore]
        public Optional<string?> Website => PersonalWebsiteUrl != null ? Optional<string?>.From(PersonalWebsiteUrl) : Optional<string?>.None();

        public string? Country { get; set; }

        [JsonIgnore]
        public Optional<string?> CountryName => Country != null ? Optional<string?>.From(Country) : Optional<string?>.None();

        public string? City { get; set; }

        [JsonIgnore]
        public Optional<string?> CityName => City != null ? Optional<string?>.From(City) : Optional<string?>.None();
    }
}
