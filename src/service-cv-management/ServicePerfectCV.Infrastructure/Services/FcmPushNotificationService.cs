using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class FcmPushNotificationService(HttpClient httpClient, IOptions<FcmSettings> options) : IPushNotificationService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly FcmSettings _settings = options.Value;

        public async Task SendAsync(IEnumerable<string> deviceTokens, string title, string message)
        {
            if (!deviceTokens.Any()) return;

            var payload = new
            {
                registration_ids = deviceTokens,
                notification = new { title, body = message }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
            request.Headers.TryAddWithoutValidation("Authorization", $"key={_settings.ServerKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
