using System;

namespace ServicePerfectCV.Application.DTOs.JobTitle.Responses
{
    public class JobTitleSuggestionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
