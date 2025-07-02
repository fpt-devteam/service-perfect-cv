using System;
using System.Collections.Generic;
using ServicePerfectCV.Application.DTOs.Category.Responses;

namespace ServicePerfectCV.Application.DTOs.Skill.Responses
{
    public class SkillResponse
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public CategoryResponse Category { get; init; } = default!;
        public List<string> Items { get; init; } = new();
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
