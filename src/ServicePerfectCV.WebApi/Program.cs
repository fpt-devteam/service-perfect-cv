using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebApi.Extensions;
using ServicePerfectCV.WebApi.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ServicePerfectCV.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load environment variables from .env file
            Env.Load();
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Setup configuration sources.
            builder.Configuration
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<RefreshTokenConfiguration>(builder.Configuration.GetSection("RefreshToken"));
            builder.Services.Configure<RefreshTokenConfiguration>(builder.Configuration.GetSection("RefreshToken"));
            builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<BaseUrlSettings>(builder.Configuration.GetSection("BaseUrlSettings"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRedis(builder.Configuration);
            builder.Services.AddConfiguredCors(builder.Configuration);
            builder.Services.AddControllers();

            builder.Services.ConfigureServices();
            builder.Services.AddAuthorizationPolicies(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            await app.Services.SeedDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // System.Console.WriteLine("hihi");
                app.UseSwagger();
                app.UseSwaggerUI();
            }
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