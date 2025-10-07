using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class Database
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("ConnectionStrings"));

            var connString = configuration.GetConnectionString("DefaultConnection");

            var dsb = new NpgsqlDataSourceBuilder(connString);
            dsb.EnableDynamicJson();
            var dataSource = dsb.Build();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(dataSource));
        }
    }
}