using ServicePerfectCV.Application.Configurations;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class RedisExtensions
    {
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));

            var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>() ?? throw new InvalidOperationException("Missing RedisSettings");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.InstanceName;
            });
        }
    }
}