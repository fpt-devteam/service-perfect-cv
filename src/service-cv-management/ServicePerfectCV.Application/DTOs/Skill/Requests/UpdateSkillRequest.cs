using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Skill.Requests
{
    public class UpdateSkillRequest
    {
        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Category must be between 1-100 characters")]
        public required string Category { get; init; }

        [Required(ErrorMessage = "Content is required.")]
        [MinLength(1, ErrorMessage = "At least one skill content is required.")]
        public required string Content { get; init; }
    }
}