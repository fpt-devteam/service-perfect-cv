using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Application.Mappings;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Infrastructure.Helpers;
using ServicePerfectCV.Infrastructure.Repositories;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using ServicePerfectCV.Infrastructure.Services;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
using ServicePerfectCV.Infrastructure.Services.Jobs;
using System.IdentityModel.Tokens.Jwt;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateExperienceRequest>();
            services.AddScoped<IFirebaseStorageService>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<FirebaseStorageSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();
                return new FirebaseStorageService(settings, logger);
            });
            services.AddAutoMapper(typeof(AuthMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));
            services.AddAutoMapper(typeof(CVMappingProfile));
            services.AddAutoMapper(typeof(EducationMappingProfile));
            services.AddAutoMapper(typeof(ContactMappingProfile));
            services.AddAutoMapper(typeof(ProjectMappingProfile));
            services.AddAutoMapper(typeof(CertificationMappingProfile));
            services.AddAutoMapper(typeof(SummaryMappingProfile));
            services.AddAutoMapper(typeof(SkillMappingProfile));
            services.AddAutoMapper(typeof(CategoryMappingProfile));
            services.AddAutoMapper(typeof(PackageMappingProfile));
            services.AddAutoMapper(typeof(BillingMappingProfile));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICVRepository, CVRepository>();
            services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IJobTitleRepository, JobTitleRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IDegreeRepository, DegreeRepository>();
            services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
            services.AddScoped<ICertificationRepository, CertificationRepository>();
            services.AddScoped<ISummaryRepository, SummaryRepository>();
            services.AddScoped<ISectionScoreResultRepository, SectionScoreResultRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IBillingHistoryRepository, BillingHistoryRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();

            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));

            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<CVService>();
            services.AddScoped<EducationService>();
            services.AddScoped<ContactService>();
            services.AddScoped<ExperienceService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<SkillService>();
            services.AddScoped<JobTitleService>();
            services.AddScoped<OrganizationService>();
            services.AddScoped<DegreeService>();

            services.AddScoped<CertificationService>();
            services.AddScoped<SummaryService>();
            services.AddScoped<EmploymentTypeService>();
            services.AddScoped<SectionScoreResultService>();
            services.AddScoped<PackageService>();
            services.AddScoped<BillingHistoryService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
            services.AddScoped<IPushNotificationService, FcmPushNotificationService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<JwtSecurityTokenHandler>();

            services.AddScoped<GoogleOAuthService>();
            services.AddScoped<LinkedInOAuthService>();
            services.AddSingleton<OAuthServiceFactory>();
            services.AddScoped<IOAuthService, GoogleOAuthService>();

            services.AddHttpClient("", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            services.AddSingleton<IJobQueue, InMemoryJobQueue>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobHandler, ScoreCvSectionHandler>();
            services.AddScoped<IJobHandler, BuildCvSectionRubricHandler>();
            services.AddScoped<JobRouter>();
            services.AddScoped<JobService>();
            services.AddHostedService<JobWorker>();

            services.AddScoped<PromptSanitizeHelper>();
            services.AddScoped<SectionRubricService>();
            services.AddScoped<SectionScoreService>();
            services.AddScoped<JobDescriptionService>();
            services.AddScoped<IJsonHelper, JsonHelper>();
            services.AddScoped<IObjectHasher, SHA256ObjectHasher>();

            services.AddScoped<IAIOrchestrator, AIOrchestrator>();

        }
    }
}