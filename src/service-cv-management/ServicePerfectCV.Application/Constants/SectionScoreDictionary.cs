using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.Constants
{
    public class SectionScoreDictionary : Dictionary<SectionType, SectionScore>
    {
        public SectionScoreDictionary() { }

        public SectionScoreDictionary(IDictionary<SectionType, SectionScore> source) : base(source) { }

        public SectionScoreDictionary(IEnumerable<KeyValuePair<SectionType, SectionScore>> pairs)
        {
            foreach (var kv in pairs) this[kv.Key] = kv.Value;
        }
    }
}