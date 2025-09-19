using System;

namespace ServicePerfectCV.Application.DTOs.Category.Responses
{
    public class CategorySuggestionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}