using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.Skill.Requests
{
    public class CreateSkillRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        public required string CategoryName { get; init; }

        [Required(ErrorMessage = "Description is required.")]
        [MinLength(1, ErrorMessage = "At least one skill description is required.")]
        public required string Description { get; init; }
    }
}