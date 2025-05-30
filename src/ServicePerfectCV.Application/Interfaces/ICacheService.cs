﻿namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}