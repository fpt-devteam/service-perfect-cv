using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class RefreshTokenService(ICacheService cache, IOptions<RefreshTokenConfiguration> options) : IRefreshTokenService
    {
        public async Task<bool> IsValidAsync(string key)
        {
            string? token = await cache.GetAsync<string>(key);
            return token != null;

        }

        public async Task<string?> GetAsync(string key)
        {
            return await cache.GetAsync<string>(key);
        }

        public async Task RevokeAsync(string key)
        {
            await cache.RemoveAsync(key);
        }

        public async Task SaveAsync(string key, string value)
        {
            await cache.SetAsync(key, value, TimeSpan.FromSeconds(options.Value.RefreshTokenExp));
        }
    }
}