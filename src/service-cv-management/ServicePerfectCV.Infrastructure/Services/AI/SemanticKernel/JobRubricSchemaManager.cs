using ServicePerfectCV.Application.Constants;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public static class JobRubricSchemaManager
    {
        public static string GetJobRubricSchema() => BuildRubricJsonSchema(SectionLimits);

        private static readonly Dictionary<CVSection, int> SectionLimits = new()
        {
            [CVSection.Contact] = 2,
            [CVSection.Summary] = 3,
            [CVSection.Skills] = 4,
            [CVSection.Experience] = 4,
            [CVSection.Projects] = 3,
            [CVSection.Education] = 3,
            [CVSection.Certifications] = 2
        };

        private static string BuildRubricJsonSchema(Dictionary<CVSection, int> sectionLimits)
        {
            var properties = string.Join(",\n",
                sectionLimits.Select(kvp => BuildSectionSchema(kvp.Key.ToString().ToLower(), kvp.Value)));

            var requiredSections = string.Join(", ",
                sectionLimits.Keys.Select(s => $"\"{s.ToString().ToLower()}\""));

            return $@"{{
            ""type"": ""object"",
            ""properties"": {{
                {properties}
            }},
            ""required"": [{requiredSections}],
            ""additionalProperties"": false
        }}";
        }

        private static string BuildSectionSchema(string sectionName, int maxItems)
        {
            return $@"""{sectionName}"": {{
                ""type"": ""object"",
                ""properties"": {{
                    ""criteria"": {{
                        ""type"": ""array"",
                        ""items"": {CriterionSchema},
                        ""maxItems"": {maxItems}
                    }}
                }},
                ""required"": [""criteria""],
                ""additionalProperties"": false
            }}";
        }

        private static readonly string CriterionSchema =
            @"{
                  ""type"": ""object"",
                    ""properties"": {
                        ""id"": {""type"": ""string""},
                        ""criterion"": {""type"": ""string""},
                        ""description"": {""type"": ""string""},
                        ""weight"": {""type"": ""number"", ""minimum"": 0, ""maximum"": 1},
                        ""scoring"": {
                            ""type"": ""object"",
                            ""properties"": {
                                ""0"": {""type"": ""string""},
                                ""1"": {""type"": ""string""},
                                ""2"": {""type"": ""string""},
                                ""3"": {""type"": ""string""},
                                ""4"": {""type"": ""string""},
                                ""5"": {""type"": ""string""}
                            },
                            ""required"": [""0"", ""1"", ""2"", ""3"", ""4"", ""5""],
                            ""additionalProperties"": false
                        }
                    },
                    ""required"": [""id"", ""criterion"", ""description"", ""weight"", ""scoring""],
                    ""additionalProperties"": false
              }";
    }
}
