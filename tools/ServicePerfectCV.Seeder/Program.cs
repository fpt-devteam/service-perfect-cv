using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.Seeder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Configuration
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            builder.Services.AddHostedService<SeedRunner>();
            builder.Services.AddTransient<DevDataSeeder>();
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            var dsb = new NpgsqlDataSourceBuilder(connString);
            dsb.EnableDynamicJson();
            var dataSource = dsb.Build();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(dataSource));

            IHost host = builder.Build();
            host.Run();
        }
    }
}