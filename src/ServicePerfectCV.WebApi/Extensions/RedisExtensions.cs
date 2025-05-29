using ServicePerfectCV.Application.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class RedisExtensions
    {
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>() ?? throw new InvalidOperationException("Missing RedisSettings");
            System.Console.WriteLine($"Redis Connection String: {redisSettings.ConnectionString}");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.InstanceName;
            });
        }

    }
}