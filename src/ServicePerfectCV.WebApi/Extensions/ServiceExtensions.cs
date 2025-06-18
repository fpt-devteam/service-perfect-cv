using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Mappings;
using ServicePerfectCV.Application.Services;
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
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<ServicePerfectCV.Application.DTOs.Experience.Requests.CreateExperienceRequest>();
            services.AddAutoMapper(typeof(AuthMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));
            services.AddAutoMapper(typeof(CVMappingProfile));
            services.AddAutoMapper(typeof(EducationMappingProfile));
            services.AddAutoMapper(typeof(ContactMappingProfile));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICVRepository, CVRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IJobTitleRepository, JobTitleRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();

            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));

            services.AddScoped<AuthService>();
            services.AddScoped<CVService>();
            services.AddScoped<EducationService>();
            services.AddScoped<ContactService>();
            services.AddScoped<ExperienceService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<JwtSecurityTokenHandler>();
        }
    }
}