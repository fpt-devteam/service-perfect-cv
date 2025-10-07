using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Infrastructure.Data;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class Database
    {
        public static void AddDatabase(this WebApplicationBuilder builder)
        {
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Build NpgsqlDataSource and ENABLE Dynamic JSON
            var dsb = new NpgsqlDataSourceBuilder(connString);
            dsb.EnableDynamicJson(); // <-- Important for POCO -> jsonb
            var dataSource = dsb.Build();

            // Register DbContext using dataSource
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(dataSource));
        }

    }
}