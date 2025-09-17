using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Infrastructure.Constants
{
    public class ScoringScale
    {
        [JsonProperty("0")]
        public string Zero { get; set; } = string.Empty;
        [JsonProperty("1")]
        public string One { get; set; } = string.Empty;
        [JsonProperty("2")]
        public string Two { get; set; } = string.Empty;
        [JsonProperty("3")]
        public string Three { get; set; } = string.Empty;
        [JsonProperty("4")]
        public string Four { get; set; } = string.Empty;
        [JsonProperty("5")]
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
