using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.Seeder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<SeedRunner>();
            builder.Services.AddTransient<DevDataSeeder>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            IHost host = builder.Build();
            host.Run();
        }
    }
}