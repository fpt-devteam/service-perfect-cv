using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Application.Mappings;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Infrastructure.Helpers;
using ServicePerfectCV.Infrastructure.Repositories;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using ServicePerfectCV.Infrastructure.Services;
using ServicePerfectCV.Infrastructure.Services.AI;
using ServicePerfectCV.Infrastructure.Services.Jobs;
using ServicePerfectCV.Infrastructure.Services.OAuth;
using System.IdentityModel.Tokens.Jwt;
using Net.payOS;

namespace ServicePerfectCV.WebAdmin.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureAdminServices(this IServiceCollection services)
        {
            // AutoMapper - Register all mapping profiles from the Application assembly
            services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICVRepository, CVRepository>();
            services.AddScoped<ICertificationRepository, CertificationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IDegreeRepository, DegreeRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IJobTitleRepository, JobTitleRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ISummaryRepository, SummaryRepository>();
            services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IBillingHistoryRepository, BillingHistoryRepository>();
            services.AddScoped<ISectionScoreResultRepository, SectionScoreResultRepository>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));

            // Firebase Storage Service - Make it optional for admin panel
            services.AddScoped<ICloudStorageService>(sp =>
            {
                try
                {
                    var settings = sp.GetRequiredService<IOptions<FirebaseStorageSettings>>().Value;
                    var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();

                    // Only initialize if credentials are properly configured
                    if (string.IsNullOrWhiteSpace(settings.ProjectId) || string.IsNullOrWhiteSpace(settings.PrivateKey))
                    {
                        logger.LogWarning("Firebase Storage credentials not configured. Cloud storage features will be disabled.");
                        return null!; // Return null for admin panel - won't be used
                    }

                    return new FirebaseStorageService(settings, logger);
                }
                catch (Exception ex)
                {
                    var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();
                    logger.LogWarning(ex, "Failed to initialize Firebase Storage. Cloud storage features will be disabled.");
                    return null!;
                }
            });

            // Application Services (only what's actually used by the admin panel)
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            // CVService removed - admin panel only reads CVs directly from database
            services.AddScoped<CertificationService>();
            services.AddScoped<ContactService>();
            services.AddScoped<DegreeService>();
            services.AddScoped<EducationService>();
            services.AddScoped<EmploymentTypeService>();
            services.AddScoped<ExperienceService>();
            services.AddScoped<JobTitleService>();
            services.AddScoped<OrganizationService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<SkillService>();
            services.AddScoped<SummaryService>();
            services.AddScoped<PackageService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<BillingHistoryService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ServicePerfectCV.Application.Services.Jobs.JobService>();
            services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
            services.AddScoped<IPushNotificationService, FcmPushNotificationService>();

            // Infrastructure Services
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<IJsonHelper, JsonHelper>();

            // Job Queue Services
            services.AddSingleton<ServicePerfectCV.Application.Interfaces.Jobs.IJobQueue, ServicePerfectCV.Infrastructure.Services.Jobs.InMemoryJobQueue>();
            services.AddScoped<ServicePerfectCV.Application.Interfaces.Jobs.IJobRepository, JobRepository>();

            // Payment Services - Configure with dummy values for admin panel (not used in admin UI)
            services.AddSingleton<PayOS>(sp =>
            {
                // Admin panel doesn't use payments, but BillingHistoryService depends on it
                // Use dummy values to satisfy DI
                return new PayOS("dummy-client-id", "dummy-api-key", "dummy-checksum-key");
            });

            // OAuth services
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

