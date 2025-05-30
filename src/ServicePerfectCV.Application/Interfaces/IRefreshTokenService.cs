using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task SaveAsync(string key, string value);
        Task RevokeAsync(string key);
        Task<bool> IsValidAsync(string key);
        Task<string?> GetAsync(string key);
    }
}