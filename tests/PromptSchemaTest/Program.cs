using System;
using System.Text.Json;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

namespace PromptSchemaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Testing JobRubricSchemaManager with Verbatim Strings ===");
            
            try
            {
                var schema = JobRubricSchemaManager.GetJobRubricSchema();
                // Console.WriteLine("Generated Schema:");
                // Console.WriteLine(schema);
                
                // Try to parse as JSON to validate
                using (JsonDocument doc = JsonDocument.Parse(schema))
                {
                    Console.WriteLine("\n✅ Schema is valid JSON!");
                    
                    // Pretty print the JSON
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string prettyJson = JsonSerializer.Serialize(doc.RootElement, options);
                    
                    Console.WriteLine("\n=== Pretty Formatted Schema ===");
                    Console.WriteLine(prettyJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
