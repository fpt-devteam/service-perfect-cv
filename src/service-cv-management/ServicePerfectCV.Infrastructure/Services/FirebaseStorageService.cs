using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly ILogger<FirebaseStorageService> _logger;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _storageUrl;

        public FirebaseStorageService(FirebaseCloudStorageSettings settings, ILogger<FirebaseStorageService> logger)
        {
            _logger = logger;
            var credentialJson = new
            {
                type = settings.Type,
                project_id = settings.ProjectId,
                private_key_id = settings.PrivateKeyId,
                private_key = settings.PrivateKey.Replace("\\n", Environment.NewLine),
                client_email = settings.ClientEmail,
                client_id = settings.ClientId,
                auth_uri = settings.AuthUri,
                token_uri = settings.TokenUri,
                auth_provider_x509_cert_url = settings.AuthProviderX509CertUrl,
                client_x509_cert_url = settings.ClientX509CertUrl,
                universe_domain = settings.UniverseDomain
            };

            // _logger.LogInformation("Credential JSON: {CredentialJson}", JsonSerializer.Serialize(credentialJson));

            var credentialJsonString = JsonSerializer.Serialize(credentialJson);
            var credential = GoogleCredential.FromJson(credentialJsonString);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            _storageClient = StorageClient.Create(credential);
            _bucketName = settings.BucketName;
            _storageUrl = settings.StorageUrl;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var fileName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
            using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(_bucketName, fileName, file.ContentType, stream);
            var encodedPath = Uri.EscapeDataString(fileName);
            return $"{_storageUrl}/{_bucketName}/o/{encodedPath}?alt=media";
        }
    }
}
