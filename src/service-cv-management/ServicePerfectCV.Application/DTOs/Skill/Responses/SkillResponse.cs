using ServicePerfectCV.Application.DTOs.Category.Responses;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Application.DTOs.Skill.Responses
{
    public class SkillResponse
    {
        public Guid Id { get; init; }
        public string Category { get; init; } = default!;
        public string Content { get; init; } = default!;
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
    }
}