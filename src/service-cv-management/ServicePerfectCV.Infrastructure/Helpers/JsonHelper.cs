using Json.Schema;
using Json.Schema.Generation;
using ServicePerfectCV.Application.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Infrastructure.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        // Default System.Text.Json settings:
        // - CamelCase property names
        // - Enums serialized as string (camelCase)
        // - Null values ignored (to keep JSON compact)
        private static readonly JsonSerializerOptions DefaultSettings = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        /// <summary>
        /// Serialize an object to JSON using System.Text.Json.
        /// </summary>
        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultSettings);
        }

        /// <summary>
        /// Deserialize JSON into an object of type T using System.Text.Json.
        /// </summary>
        public T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultSettings);
        }

        /// <summary>
        /// Deserialize JSON into an object with a runtime type using System.Text.Json.
        /// </summary>
        public object? Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, DefaultSettings);
        }

        // =========================
        // JSON SCHEMA (JsonSchema.Net)
        // =========================

        /// <summary>
        /// Generate JSON Schema from a generic type (T) using JsonSchema.Net.
        /// </summary>
        public string GenerateJsonSchema<T>()
        {
            var schema = new JsonSchemaBuilder().FromType<T>().Build();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(schema, options);
        }

        /// <summary>
        /// Generate JSON Schema from a runtime type using JsonSchema.Net.
        /// </summary>
        public string GenerateJsonSchema(Type type)
        {
            var schema = new JsonSchemaBuilder().FromType(type).Build();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(schema, options);
        }
    }
}
