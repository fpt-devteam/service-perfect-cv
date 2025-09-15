using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServicePerfectCV.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }

        public static string Serialize<T>(T obj, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
        }
    }
}
