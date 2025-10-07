using ServicePerfectCV.Application.Configurations;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CorsSettings>(configuration.GetSection("CorsSettings"));

            var corsOptions = configuration
                .GetSection("CorsSettings")
                .Get<CorsSettings>()
                ?? throw new InvalidOperationException("Missing CorsSettings");

            services.AddCors(options =>
            {
                options.AddPolicy("AppCorsPolicy", policyBuilder =>
                {
                    policyBuilder
                        .WithOrigins(corsOptions.AllowedOrigins)
                        .WithHeaders(corsOptions.AllowedHeaders)
                        .WithMethods(corsOptions.AllowedMethods)
                        .WithExposedHeaders(corsOptions.ExposedHeaders);

                    if (corsOptions.AllowCredentials)
                        policyBuilder.AllowCredentials();
                    else
                        policyBuilder.DisallowCredentials();

                    policyBuilder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsOptions.PreflightMaxAge));
                });
            });

            return services;
        }
    }
}