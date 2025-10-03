using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Mappings;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Infrastructure.Helpers;
using ServicePerfectCV.Infrastructure.Repositories;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using ServicePerfectCV.Infrastructure.Services;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
using System.IdentityModel.Tokens.Jwt;

namespace ServicePerfectCV.WebAdmin.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureAdminServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(AuthMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));

            // Firebase Storage Service
            services.AddScoped<IFirebaseStorageService>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<FirebaseStorageSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();
                return new FirebaseStorageService(settings, logger);
            });

            // Services
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<IJsonHelper, JsonHelper>();

            // OAuth services (if needed for admin login via OAuth)
            services.AddScoped<GoogleOAuthService>();
            services.AddScoped<LinkedInOAuthService>();
            services.AddSingleton<OAuthServiceFactory>();
            services.AddScoped<IOAuthService, GoogleOAuthService>();

            services.AddHttpClient("", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
            });
        }
    }
}

