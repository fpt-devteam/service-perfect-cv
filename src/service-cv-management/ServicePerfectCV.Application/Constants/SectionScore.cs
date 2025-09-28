using System.Text.Json.Serialization;

namespace ServicePerfectCV.Application.Constants
{
    public sealed class SectionScore
    {
        public List<CriteriaScore> CriteriaScores { get; set; } = new();
        public int TotalScore0To5 { get; set; } = 0;
        public double Weight0To1 { get; set; } = 0;
    }

    public sealed class CriteriaScore
    {
        public string Id { get; set; } = string.Empty;
        public string Criterion { get; set; } = string.Empty;
        public int Score0To5 { get; set; } = 0;
        public double Weight0To1 { get; set; } = 0;
        public string Justification { get; set; } = string.Empty;
        public List<string> EvidenceFound { get; set; } = new();
        public List<string> MissingElements { get; set; } = new();
    }
}