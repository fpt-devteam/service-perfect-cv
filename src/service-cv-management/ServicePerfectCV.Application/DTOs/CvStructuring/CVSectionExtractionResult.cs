using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Application.DTOs.CvStructuring
{
    /// <summary>
    /// Result of CV section extraction from raw text
    /// </summary>
    public sealed class CvSectionExtractionResult
    {
        /// <summary>
        /// Dictionary mapping section types to their extracted JSON data
        /// </summary>
        public Dictionary<SectionType, JsonDocument> Sections { get; set; } = new();

        /// <summary>
        /// Any errors encountered during extraction
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Indicates if the extraction was successful
        /// </summary>
        public bool IsSuccess => Errors.Count == 0 && Sections.Count > 0;
    }
}
