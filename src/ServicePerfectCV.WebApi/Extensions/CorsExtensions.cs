using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using System;
using System.Linq;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOptions = configuration
                .GetSection("CorsSettings")
                .Get<CorsSettings>()
                ?? throw new InvalidOperationException("Missing CorsSettings");

            // Convert configuration comma-separated strings into arrays.
            var allowedOrigins = corsOptions.AllowedOrigins.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var allowedHeaders = corsOptions.AllowedHeaders.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var allowedMethods = corsOptions.AllowedMethods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var exposedHeaders = corsOptions.ExposedHeaders.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Log the CORS settings to the console for debugging.
            Console.WriteLine($"CORS Settings: AllowedOrigins={string.Join(",", allowedOrigins)}, " +
                              $"AllowedHeaders={string.Join(",", allowedHeaders)}, " +
                              $"AllowedMethods={string.Join(",", allowedMethods)}, " +
                              $"ExposedHeaders={string.Join(",", exposedHeaders)}, " +
                              $"AllowCredentials={corsOptions.AllowCredentials}, " +
                              $"PreflightMaxAge={corsOptions.PreflightMaxAge}");

            services.AddCors(options =>
            {
                options.AddPolicy("AppCorsPolicy", policyBuilder =>
                {
                    policyBuilder
                        .WithOrigins(allowedOrigins)
                        .WithHeaders(allowedHeaders)
                        .WithMethods(allowedMethods)
                        .WithExposedHeaders(exposedHeaders);

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