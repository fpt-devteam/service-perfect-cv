using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
    }
}
