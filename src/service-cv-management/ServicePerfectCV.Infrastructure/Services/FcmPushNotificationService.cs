using Google.Apis.Auth.OAuth2;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class FcmPushNotificationService(HttpClient httpClient, IOptions<FcmSettings> options, ILogger<FcmPushNotificationService> logger) : IPushNotificationService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly FcmSettings _settings = options.Value;
        private readonly ILogger<FcmPushNotificationService> _logger = logger;
        private GoogleCredential? _credential;

        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                _logger.LogInformation("Getting FCM access token for project: {ProjectId}", _settings.ProjectId);

                if (_credential == null)
                {
                    var credentialJson = new
                    {
                        type = _settings.Type,
                        project_id = _settings.ProjectId,
                        private_key_id = _settings.PrivateKeyId,
                        private_key = _settings.PrivateKey.Replace("\\n", Environment.NewLine),
                        client_email = _settings.ClientEmail,
                        client_id = _settings.ClientId,
                        auth_uri = _settings.AuthUri,
                        token_uri = _settings.TokenUri,
                        auth_provider_x509_cert_url = _settings.AuthProviderX509CertUrl,
                        client_x509_cert_url = _settings.ClientX509CertUrl,
                        universe_domain = _settings.UniverseDomain
                    };

                    // _logger.LogInformation("Credential JSON: {CredentialJson}", JsonSerializer.Serialize(credentialJson));

                    var credentialJsonString = JsonSerializer.Serialize(credentialJson);
                    _credential = GoogleCredential
                        .FromJson(credentialJsonString)
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                }

                var token = await ((ITokenAccess)_credential).GetAccessTokenForRequestAsync("https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send", CancellationToken.None);

                _logger.LogInformation("Successfully obtained FCM access token");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get FCM access token");
                throw;
            }
        }

        public async Task SendAsync(IEnumerable<string> deviceTokens, string title, string message)
        {
            _logger.LogInformation("Sending push notification to {Count} devices", deviceTokens.Count());

            foreach (var token in deviceTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Skipping empty device token");
                    continue;
                }

                try
                {
                    var payload = new
                    {
                        message = new
                        {
                            token,
                            notification = new { title, body = message }
                        }
                    };

                    var fcmUrl = $"https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send";
                    _logger.LogInformation("Sending to FCM URL: {Url}", fcmUrl);

                    using var request = new HttpRequestMessage(HttpMethod.Post, fcmUrl);
                    var accessToken = await GetAccessTokenAsync();
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                    _logger.LogInformation("Sending FCM request for token: {Token}", token.Substring(0, Math.Min(10, token.Length)) + "...");

                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("FCM request failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                        throw new HttpRequestException($"FCM request failed: {response.StatusCode} - {errorContent}");
                    }

                    _logger.LogInformation("Successfully sent notification to device token: {Token}", token.Substring(0, Math.Min(10, token.Length)) + "...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send notification to device token: {Token}", token.Substring(0, Math.Min(10, token.Length)) + "...");
                    throw;
                }
            }
        }
    }
}
