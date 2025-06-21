using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories;
using ServicePerfectCV.Infrastructure.Services;
using ServicePerfectCV.WebApi;

namespace ServicePerfectCV.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            builder.ConfigureServices(services =>
            {
                var dbContextOptions = services.Where(s =>
                    s.ServiceType.Name.Contains("DbContextOptions") ||
                    (s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                    .ToList();

                foreach (var options in dbContextOptions)
                {
                    services.Remove(options);
                }

                var repositories = services.Where(s => s.ServiceType.Name.Contains("Repository")).ToList();
                foreach (var repo in repositories)
                {
                    services.Remove(repo);
                }

                var redisDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICacheService));
                if (redisDescriptor != null)
                {
                    services.Remove(redisDescriptor);
                }

                var dbName = $"InMemoryTestDb_{Guid.NewGuid()}";
                
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                });

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<ICVRepository, CVRepository>();
                services.AddScoped<IContactRepository, ContactRepository>();
                services.AddScoped<IExperienceRepository, ExperienceRepository>();
                services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
                services.AddScoped<IJobTitleRepository, JobTitleRepository>();
                services.AddScoped<IOrganizationRepository, OrganizationRepository>();
                services.AddScoped<IProjectRepository, ProjectRepository>();
                services.AddScoped<IEducationRepository, EducationRepository>();
                services.AddScoped<IDegreeRepository, DegreeRepository>();
                services.AddScoped<ITokenGenerator, TokenGenerator>();

                services.AddSingleton<ICacheService>(sp =>
                {
                    var mock = new Mock<ICacheService>();
                    return mock.Object;
                });
            });
        }

        private IServiceProvider? _services;
        public new IServiceProvider Services => _services ??= Server.Services;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<ApplicationDbContext>();
                var logger = services.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                try
                {
                    db.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error when start: {Message}", ex.Message);
                }
            }

            return host;
        }
    }
}
