using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
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

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RefreshTokenConfiguration>(configuration.GetSection("RefreshToken"));
            services.Configure<DatabaseSettings>(configuration.GetSection("ConnectionStrings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<UrlSettings>(configuration.GetSection("UrlSettings"));
            services.Configure<FcmSettings>(configuration.GetSection("FcmSettings"));
            services.Configure<FirebaseStorageSettings>(configuration.GetSection("FirebaseStorageSettings"));

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateExperienceRequest>();
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

            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateHelper, EmailTemplateHelper>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
            services.AddScoped<IPushNotificationService, FcmPushNotificationService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<ICloudStorageService>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<FirebaseStorageSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<FirebaseStorageService>>();
                return new FirebaseStorageService(settings, logger);
            });

            services.AddSingleton<OAuthServiceFactory>();
            services.AddScoped<IOAuthService, GoogleOAuthService>();

            services.AddSingleton<IJobQueue, InMemoryJobQueue>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobHandler, ScoreCvSectionHandler>();
            services.AddScoped<IJobHandler, BuildCvSectionRubricHandler>();
            services.AddScoped<IJobHandler, StructureCvContentHandler>();
            services.AddScoped<JobRouter>();
            services.AddScoped<JobService>();
            services.AddHostedService<JobWorker>();

            services.AddScoped<PromptSanitizeHelper>();
            services.AddScoped<ISectionRubricService, SectionRubricService>();
            services.AddScoped<ISectionScoreService, SectionScoreService>();
            services.AddScoped<SectionRubricService>();
            services.AddScoped<SectionScoreService>();
            services.AddScoped<JobDescriptionService>();
            services.AddScoped<IJsonHelper, JsonHelper>();
            services.AddScoped<IObjectHasher, SHA256ObjectHasher>();

            services.AddScoped<IOCRService, OCRService>();
            services.AddScoped<ICvStructuringService, CvStructuringService>();

        }
    }
}