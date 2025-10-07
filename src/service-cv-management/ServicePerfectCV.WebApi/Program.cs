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

            WebApplication app = builder.Build();
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
    }
}