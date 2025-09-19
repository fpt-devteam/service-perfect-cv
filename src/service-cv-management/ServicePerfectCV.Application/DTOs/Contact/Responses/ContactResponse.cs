using System;

namespace ServicePerfectCV.Application.DTOs.Contact.Responses
{
    public class ContactResponse
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
        public string? LinkedInUrl { get; init; }
        public string? GitHubUrl { get; init; }
        public string? PersonalWebsiteUrl { get; init; }
        public string? Country { get; init; }
        public string? City { get; init; }
    }
}