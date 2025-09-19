using System;

namespace ServicePerfectCV.Application.DTOs.Degree.Responses
{
    public class DegreeSuggestionResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}