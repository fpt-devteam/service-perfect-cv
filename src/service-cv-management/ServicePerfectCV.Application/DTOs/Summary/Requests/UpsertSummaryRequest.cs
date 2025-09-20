using ServicePerfectCV.Application.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.Summary.Requests
{
    public class UpsertSummaryRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10-2000 characters")]
        public string? Content { get; set; }

        [JsonIgnore]
        public Optional<string?> SummaryContent => Content != null ? Optional<string?>.From(Content) : Optional<string?>.None();
    }
}