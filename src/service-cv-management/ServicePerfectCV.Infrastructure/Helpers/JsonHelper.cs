using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace ServicePerfectCV.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        // Default Newtonsoft.Json settings:
        // - CamelCase property names
        // - Enums serialized as string (camelCase)
        // - Null values ignored (to keep JSON compact)
        private static readonly JsonSerializerSettings DefaultSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            Converters =
            {
                new StringEnumConverter(new CamelCaseNamingStrategy())
            }
        };

        /// <summary>
        /// Serialize an object to JSON using Newtonsoft.Json.
        /// </summary>
        public static string Serialize<T>(T obj, JsonSerializerSettings? settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings ?? DefaultSettings);
        }

        /// <summary>
        /// Deserialize JSON into an object of type T using Newtonsoft.Json.
        /// </summary>
        public static T? Deserialize<T>(string json, JsonSerializerSettings? settings = null)
        {
            return JsonConvert.DeserializeObject<T>(json, settings ?? DefaultSettings);
        }

        /// <summary>
        /// Deserialize JSON into an object with a runtime type using Newtonsoft.Json.
        /// </summary>
        public static object? Deserialize(string json, Type type, JsonSerializerSettings? settings = null)
        {
            return JsonConvert.DeserializeObject(json, type, settings ?? DefaultSettings);
        }

        // =========================
        // JSON SCHEMA (Newtonsoft.Json.Schema)
        // =========================

        // Creates a default JSchemaGenerator configuration:
        // - Required.Default: properties are optional unless explicitly marked
        // - Stable SchemaId generation
        // - Enums as string values to match StringEnumConverter
        private static JSchemaGenerator CreateDefaultSchemaGenerator()
        {
            var generator = new JSchemaGenerator
            {
                // Change to Required.Always if you want every property to be required
                DefaultRequired = Required.Default,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.None
            };

            // Ensure enums are generated as string values
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            // Attributes such as [JsonProperty], [JsonRequired], [DefaultValue], etc.
            // will be respected automatically by JSchemaGenerator
            return generator;
        }

        /// <summary>
        /// Generate JSON Schema from a generic type (T) using Newtonsoft.Json.Schema.
        /// </summary>
        public static string GenerateJsonSchema<T>(Action<JSchemaGenerator>? configure = null)
        {
            var generator = CreateDefaultSchemaGenerator();
            configure?.Invoke(generator);

            var schema = generator.Generate(typeof(T));
            return schema.ToString();
        }

        /// <summary>
        /// Generate JSON Schema from a runtime type using Newtonsoft.Json.Schema.
        /// </summary>
        public static string GenerateJsonSchema(Type type, Action<JSchemaGenerator>? configure = null)
        {
            var generator = CreateDefaultSchemaGenerator();
            configure?.Invoke(generator);

            var schema = generator.Generate(type);
            return schema.ToString();
        }
    }

    /// <summary>
    /// Custom provider to generate enum values as strings in JSON Schema,
    /// matching StringEnumConverter with camelCase naming.
    /// </summary>
    internal sealed class StringEnumGenerationProvider : JSchemaGenerationProvider
    {
        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            // Enum -> string type with allowed enum values
            var schema = new JSchema
            {
                Type = JSchemaType.String
            };

            var names = Enum.GetNames(context.ObjectType);
            foreach (var name in names)
            {
                // Convert enum names to camelCase (to match serialization)
                var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
                schema.Enum.Add(new JValue(camel));
            }

            return schema;
        }

        public override bool CanGenerateSchema(JSchemaTypeGenerationContext context)
        {
            return context.ObjectType.IsEnum;
        }
    }
}
