using Microsoft.AspNetCore.Http;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
    }
}