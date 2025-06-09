using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebApi;
using System;
using System.Linq;

namespace ServerPerfectCV.WebApi.IntegrationTests.TestBase
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Remove the app's ApplicationDbContext registration
            builder.ConfigureServices(services =>
            {
                // Find services registered as DbContextOptions<ApplicationDbContext> and remove them
                var descriptor = services.FirstOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Find services registered as ApplicationDbContext and remove them
                var appDbContextDescriptor = services.FirstOrDefault(d =>
                    d.ServiceType == typeof(ApplicationDbContext));
                if (appDbContextDescriptor != null)
                {
                    services.Remove(appDbContextDescriptor);
                }

                // Remove any DbContext registrations
                var dbContextOptionsDescriptor = services.FirstOrDefault(d =>
                    d.ServiceType.Name.Contains("DbContextOptions"));
                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // Add ApplicationDbContext using InMemory Database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDb");
                });

                // Add test authentication
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            // Configure the environment
            builder.UseEnvironment("Testing");
        }
    }
}