using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.Skill.Requests
{
    public class CreateSkillRequest
    {
        [Required(ErrorMessage = "Category is required")]
        public required string Category { get; init; }

        [Required(ErrorMessage = "Content is required.")]
        [MinLength(1, ErrorMessage = "At least one skill content is required.")]
        public required string Content { get; init; }
    }
}