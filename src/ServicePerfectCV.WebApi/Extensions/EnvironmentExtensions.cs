using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class EnvironmentExtensions
    {
        public static void LoadEnvironmentVariables(this IServiceCollection services)
        {
            // Load environment variables
            Env.Load();

            // Build connection string
            var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_SERVER")},{Environment.GetEnvironmentVariable("DB_PORT")};" +
                                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                   $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                   $"TrustServerCertificate={Environment.GetEnvironmentVariable("DB_TRUST_CERTIFICATE")};";

            // Add DbContext to the service collection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
