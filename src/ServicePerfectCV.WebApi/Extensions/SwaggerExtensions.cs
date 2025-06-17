using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => 
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Service Perfect CV API",
                    Version = "v1",
                    Description = "API for Service Perfect CV application"
                });
            });
            
            return services;
        }
    }
}