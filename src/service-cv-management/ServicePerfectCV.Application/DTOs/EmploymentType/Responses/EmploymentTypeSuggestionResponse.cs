using System;

namespace ServicePerfectCV.Application.DTOs.EmploymentType.Responses
{
    public class EmploymentTypeSuggestionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}