using System;
using System.Collections.Generic;
using ServicePerfectCV.Application.DTOs.Category.Responses;

namespace ServicePerfectCV.Application.DTOs.Skill.Responses
{
    public class SkillResponse
    {
        public Guid Id { get; init; }
        public string Category { get; init; } = default!;
        public string Description { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
