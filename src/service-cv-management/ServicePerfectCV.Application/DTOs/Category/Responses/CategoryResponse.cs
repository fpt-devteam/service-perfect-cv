using System;

namespace ServicePerfectCV.Application.DTOs.Category.Responses
{
    public class CategoryResponse
    {
        public required string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}