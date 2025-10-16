using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.DependencyInjections;
using ServicePerfectCV.WebApi.Extensions;
using ServicePerfectCV.WebApi.Middleware;

namespace ServicePerfectCV.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddRedis(builder.Configuration);
            builder.Services.AddConfiguredCors(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddJsonNamingConfiguration();
            builder.Services.AddServiceConfigurations(builder.Configuration);
            builder.Services.AddAuthWithJwtAndGoogle(builder.Configuration);
            builder.Services.AddSemanticKernelInfra(builder.Configuration);
            builder.Services.AddAzureAIFoundry(builder.Configuration);
            builder.Services.AddConfiguredSwagger();
            builder.Services.AddPayOS(builder.Configuration);

            WebApplication app = builder.Build();

            // Run database migrations automatically on startup
            await MigrateDatabaseAsync(app);

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AppCorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseConfiguredStaticFiles();
            app.MapControllers();

            await app.RunAsync();
        }

        private static async Task MigrateDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Starting database migration...");

                // Ensure database is created and apply pending migrations
                await context.Database.MigrateAsync();

                logger.LogInformation("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
                throw;
            }
        }
    }
}