using System;

namespace ServicePerfectCV.Application.DTOs.Category.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
