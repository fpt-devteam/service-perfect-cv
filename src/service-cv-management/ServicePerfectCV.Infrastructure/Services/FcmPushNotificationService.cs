using Google.Apis.Auth.OAuth2;
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
        private GoogleCredential? _credential;

        private async Task<string> GetAccessTokenAsync()
        {
            _credential ??= GoogleCredential
                .FromFile(_settings.ServiceAccountKeyPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            return await _credential.GetAccessTokenForRequestAsync();
        }

        public async Task SendAsync(IEnumerable<string> deviceTokens, string title, string message)
        {
            foreach (var token in deviceTokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;

                var payload = new
                {
                    message = new
                    {
                        token,
                        notification = new { title, body = message }
                    }
                };

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
