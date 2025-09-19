using System;

namespace ServicePerfectCV.Application.DTOs.Organization.Responses
{
    public class OrganizationSuggestionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string OrganizationType { get; set; } = default!;
    }
}