using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Application.Extensions
{
    public static class SectionRubricDictionaryExtensions
    {
        /// <summary>
        /// Converts a SectionRubricDictionary to a Dictionary&lt;SectionType, string&gt;
        /// where the string is the JSON serialization of the SectionRubric.
        /// </summary>
        public static Dictionary<SectionType, string> ToSerializedDictionary(this SectionRubricDictionary rubricDict, IJsonHelper jsonHelper)
        {
            var result = new Dictionary<SectionType, string>();

            foreach (var kvp in rubricDict)
            {
                result[kvp.Key] = jsonHelper.Serialize(kvp.Value);
            }

            return result;
        }
    }
}