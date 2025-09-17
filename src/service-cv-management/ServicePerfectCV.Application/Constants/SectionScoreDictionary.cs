namespace ServicePerfectCV.Application.Constants
{
    public class SectionScoreDictionary : Dictionary<Section, SectionScore>
    {
        public SectionScoreDictionary() { }

        public SectionScoreDictionary(IDictionary<Section, SectionScore> source) : base(source) { }

        public SectionScoreDictionary(IEnumerable<KeyValuePair<Section, SectionScore>> pairs)
        {
            foreach (var kv in pairs) this[kv.Key] = kv.Value;
        }
    }
}
