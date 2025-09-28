using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Domain.ValueObjects
{

    public class ScoringScale
    {
        [JsonPropertyName("0")]
        public string Zero { get; set; } = string.Empty;
        [JsonPropertyName("1")]
        public string One { get; set; } = string.Empty;
        [JsonPropertyName("2")]
        public string Two { get; set; } = string.Empty;
        [JsonPropertyName("3")]
        public string Three { get; set; } = string.Empty;
        [JsonPropertyName("4")]
        public string Four { get; set; } = string.Empty;
        [JsonPropertyName("5")]
        public string Five { get; set; } = string.Empty;
    }

    public class SectionRubricCriteria
    {
        public string Id { get; set; } = default!;
        public string Criterion { get; set; } = default!;
        public string Description { get; set; } = default!;
        [Range(0, 1)]
        public double Weight0To1 { get; set; }
        public ScoringScale Scoring { get; set; } = new();
    }

    public class SectionRubric
    {
        [Range(0, 1)]
        public double Weight0To1 { get; set; }
        [MaxLength(4)]
        public List<SectionRubricCriteria> Criteria { get; set; } = new();
    }
}
