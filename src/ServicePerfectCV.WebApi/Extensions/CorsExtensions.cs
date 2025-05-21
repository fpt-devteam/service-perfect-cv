using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration, string policyName = "DevelopmentPolicy")
        {
            // Retrieve CORS configuration from appsettings.json or appsettings.Development.json
            var corsSettings = configuration.GetSection("Cors");

            var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>();
            var allowedMethods = corsSettings.GetSection("AllowedMethods").Get<string[]>();
            var allowedHeaders = corsSettings.GetSection("AllowedHeaders").Get<string[]>();
            var exposedHeaders = corsSettings.GetSection("ExposedHeaders").Get<string[]>();
            var allowCredentials = corsSettings.GetValue<bool>("AllowCredentials");
            var preflightMaxAge = corsSettings.GetValue<int>("PreflightMaxAge");

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    // Configure allowed origins
                    if (allowedOrigins!.Contains("*"))
                        builder.AllowAnyOrigin();
                    else
                        builder.WithOrigins(allowedOrigins!);

                    // Configure allowed methods
                    if (allowedMethods!.Contains("*"))
                        builder.AllowAnyMethod();
                    else
                        builder.WithMethods(allowedMethods!);

                    // Configure allowed headers
                    if (allowedHeaders!.Contains("*"))
                        builder.AllowAnyHeader();
                    else
                        builder.WithHeaders(allowedHeaders!);

                    // Configure exposed headers if available
                    if (exposedHeaders != null && exposedHeaders.Length > 0)
                        builder.WithExposedHeaders(exposedHeaders);

                    // Configure allow credentials
                    if (allowCredentials)
                        builder.AllowCredentials();

                    // Configure preflight max age
                    builder.SetPreflightMaxAge(TimeSpan.FromSeconds(preflightMaxAge));
                });
            });

            return services;
        }
    }
}