using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Application.Extensions
{
    public static class SectionRubricDictionaryExtensions
    {
        /// <summary>
        /// Converts a SectionRubricDictionary to a Dictionary&lt;Section, string&gt;
        /// where the string is the JSON serialization of the SectionRubric.
        /// </summary>
        public static Dictionary<Section, string> ToSerializedDictionary(this SectionRubricDictionary rubricDict, IJsonHelper jsonHelper)
        {
            var result = new Dictionary<Section, string>();

            foreach (var kvp in rubricDict)
            {
                result[kvp.Key] = jsonHelper.Serialize(kvp.Value);
            }

            return result;
        }
    }
}