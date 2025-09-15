using ServicePerfectCV.Application.Constants;

namespace ServicePerfectCV.Infrastructure.Constants
{
    public class SectionRubricCriteria
    {
        public string Id { get; set; } = default!;
        public string Criterion { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double Weight { get; set; }

        public Dictionary<string, string> Scoring { get; set; } = new();
    }

    public class SectionRubric
    {
        public List<SectionRubricCriteria> Criteria { get; set; } = new();
    }

    public class JobRubric : Dictionary<CVSection, SectionRubric> { }
}
