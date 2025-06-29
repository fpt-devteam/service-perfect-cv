using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebApi.Extensions;
using ServicePerfectCV.WebApi.Middleware;

namespace ServicePerfectCV.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<RefreshTokenConfiguration>(builder.Configuration.GetSection("RefreshToken"));
            builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<UrlSettings>(builder.Configuration.GetSection(" UrlSettings"));
            builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRedis(builder.Configuration);
            builder.Services.AddConfiguredCors(builder.Configuration);
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            builder.Services.ConfigureServices();
            builder.Services.AddAuthWithJwtAndGoogle(builder.Configuration);
            // builder.Services.AddAuthentication(builder.Configuration);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddConfiguredSwagger();

            WebApplication app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (!app.Environment.IsEnvironment("Testing"))
            {
                using (IServiceScope scope = app.Services.CreateScope())
                {
                    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await dbContext.Database.MigrateAsync();
                }
            }

            //TODO: uncomment when db schema is ready
            // await app.Services.SeedDatabaseAsync();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AppCorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
                RequestPath = "/templates"
            });
            app.MapControllers();

            await app.RunAsync();
        }
    }
}