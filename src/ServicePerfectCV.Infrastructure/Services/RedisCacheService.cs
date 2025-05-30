using Microsoft.Extensions.Caching.Distributed;
using ServicePerfectCV.Application.Interfaces;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class RedisCacheService(IDistributedCache distributedCache) : ICacheService
    {
        public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = ttl
            };
            await (distributedCache?.SetStringAsync(key, JsonSerializer.Serialize(value), options) ?? Task.CompletedTask);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            string? value = await distributedCache.GetStringAsync(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task RemoveAsync(string key)
        {
            await distributedCache.RemoveAsync(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}