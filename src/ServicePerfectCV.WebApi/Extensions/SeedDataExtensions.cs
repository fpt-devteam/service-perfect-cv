using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Data.Seeding;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class SeedDataExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            var seedEnabled = config.GetValue<bool>("Seed:Enabled");
            if (!seedEnabled)
                return;
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                logger.LogInformation("Starting database seeding...");
                // await context.Database.EnsureCreatedAsync();
                await context.Database.MigrateAsync();
                await ApplicationDbContextSeed.SeedAsync(context);
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
