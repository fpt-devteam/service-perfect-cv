using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _storageUrl;

        public FirebaseStorageService(FirebaseSettings settings)
        {
            var credential = GoogleCredential.FromFile(settings.CredentialsPath);

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
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{encodedPath}?alt=media";
        }
    }
}
