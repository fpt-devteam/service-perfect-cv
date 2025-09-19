using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Skill.Requests
{
    public class UpdateSkillRequest
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Category name must be between 1-100 characters")]
        public required string CategoryName { get; init; }

        [Required(ErrorMessage = "Description is required.")]
        [MinLength(1, ErrorMessage = "At least one skill description is required.")]
        public required string Description { get; init; }
    }
}