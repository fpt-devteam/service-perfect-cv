using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;
namespace PromptSchemaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Testing Enhanced JobRubricSchemaManager and SectionScoringSchemaManager ===");

            TestJobRubricSchema();
            // TestSectionScoringSchema();
        }

        private static void TestJobRubricSchema()
        {
            Console.WriteLine("\n📋 Testing Job Rubric Schema Generation...");

            try
            {
                var schema = JsonHelper.GenerateJsonSchema<SectionRubricDictionary>();

                // Try to parse as JSON to validate
                using (JsonDocument doc = JsonDocument.Parse(schema))
                {
                    Console.WriteLine("✅ Job Rubric Schema is valid JSON!");

                    // Check structure
                    var root = doc.RootElement;
                    if (root.TryGetProperty("properties", out var properties))
                    {
                        Console.WriteLine($"📊 Schema contains {properties.EnumerateObject().Count()} CV sections:");
                        foreach (var section in properties.EnumerateObject())
                        {
                            Console.WriteLine($"   - {section.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Job Rubric Schema Error: {ex.Message}");
            }
        }

        private static void TestSectionScoringSchema()
        {
            Console.WriteLine("\n🎯 Testing Section Scoring Schema...");

            try
            {
                var schema = JsonHelper.GenerateJsonSchema<SectionRubricDictionary>();

                // Try to parse as JSON to validate
                using (JsonDocument doc = JsonDocument.Parse(schema))
                {
                    Console.WriteLine("✅ Section Scoring Schema is valid JSON!");

                    // Pretty print for verification
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string prettyJson = JsonSerializer.Serialize(doc.RootElement, options);

                    Console.WriteLine("\n=== Section Scoring Schema Structure ===");
                    Console.WriteLine(prettyJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Section Scoring Schema Error: {ex.Message}");
            }
        }
    }
}
