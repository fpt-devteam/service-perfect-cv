using ServicePerfectCV.Application.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.Summary.Requests
{
    public class UpsertSummaryRequest
    {
        [Required(ErrorMessage = "CV ID is required.")]
        public required Guid CVId { get; init; }

        [Required(ErrorMessage = "Context is required.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Context must be between 10-2000 characters")]
        public string? Context { get; set; }

        [JsonIgnore]
        public Optional<string?> SummaryContext => Context != null ? Optional<string?>.From(Context) : Optional<string?>.None();
    }
}