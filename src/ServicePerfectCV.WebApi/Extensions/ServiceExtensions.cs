using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Mappings;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Validators;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Helpers;
using ServicePerfectCV.Infrastructure.Repositories;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using ServicePerfectCV.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AuthMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));

            // register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();

            // register generic repository
            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));

            // register services
            services.AddScoped<AuthService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<JwtSecurityTokenHandler>();

            // register validators
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        }
    }
}