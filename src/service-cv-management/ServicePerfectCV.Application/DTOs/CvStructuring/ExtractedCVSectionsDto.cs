using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.DTOs.CvStructuring
{
    /// <summary>
    /// DTO representing all extracted CV sections from LLM response
    /// </summary>
    public sealed class ExtractedCVSectionsDto
    {
        [JsonPropertyName("contact")]
        public ExtractedContactDto? Contact { get; set; }

        [JsonPropertyName("summary")]
        public ExtractedSummaryDto? Summary { get; set; }

        [JsonPropertyName("education")]
        public List<ExtractedEducationDto>? Education { get; set; }

        [JsonPropertyName("experience")]
        public List<ExtractedExperienceDto>? Experience { get; set; }

        [JsonPropertyName("skills")]
        public List<ExtractedSkillDto>? Skills { get; set; }

        [JsonPropertyName("projects")]
        public List<ExtractedProjectDto>? Projects { get; set; }

        [JsonPropertyName("certifications")]
        public List<ExtractedCertificationDto>? Certifications { get; set; }
    }

    public sealed class ExtractedContactDto
    {
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("linkedInUrl")]
        public string? LinkedInUrl { get; set; }

        [JsonPropertyName("gitHubUrl")]
        public string? GitHubUrl { get; set; }

        [JsonPropertyName("personalWebsiteUrl")]
        public string? PersonalWebsiteUrl { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }
    }

    public sealed class ExtractedSummaryDto
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public sealed class ExtractedEducationDto
    {
        [JsonPropertyName("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName("degree")]
        public string? Degree { get; set; }

        [JsonPropertyName("fieldOfStudy")]
        public string? FieldOfStudy { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("gpa")]
        public string? Gpa { get; set; }
    }

    public sealed class ExtractedExperienceDto
    {
        [JsonPropertyName("jobTitle")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("employmentType")]
        public string? EmploymentType { get; set; }

        [JsonPropertyName("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public sealed class ExtractedSkillDto
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public sealed class ExtractedProjectDto
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
    }

    public sealed class ExtractedCertificationDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName("issuedDate")]
        public DateTime? IssuedDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
