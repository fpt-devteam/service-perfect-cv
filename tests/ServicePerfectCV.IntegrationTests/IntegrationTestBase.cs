using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Domain.Constants;
using Xunit;
using ServicePerfectCV.Infrastructure.Repositories;

namespace ServicePerfectCV.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        protected readonly HttpClient Client;
        protected readonly IUserRepository UserRepository;
        protected readonly ICVRepository CVRepository;
        protected readonly IContactRepository ContactRepository;
        protected readonly IExperienceRepository ExperienceRepository;
        protected readonly IEmploymentTypeRepository EmploymentTypeRepository;
        protected readonly IJobTitleRepository JobTitleRepository;
        protected readonly IOrganizationRepository OrganizationRepository;
        protected readonly IProjectRepository ProjectRepository;
        protected readonly IEducationRepository EducationRepository;
        protected readonly IDegreeRepository DegreeRepository;
        protected readonly ICertificationRepository CertificationRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _dbContext;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();
            _scope = factory.Services.CreateScope();
            UserRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
            CVRepository = _scope.ServiceProvider.GetRequiredService<ICVRepository>();
            ContactRepository = _scope.ServiceProvider.GetRequiredService<IContactRepository>();
            ExperienceRepository = _scope.ServiceProvider.GetRequiredService<IExperienceRepository>();
            EmploymentTypeRepository = _scope.ServiceProvider.GetRequiredService<IEmploymentTypeRepository>();
            JobTitleRepository = _scope.ServiceProvider.GetRequiredService<IJobTitleRepository>();
            OrganizationRepository = _scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();
            ProjectRepository = _scope.ServiceProvider.GetRequiredService<IProjectRepository>();
            EducationRepository = _scope.ServiceProvider.GetRequiredService<IEducationRepository>();
            DegreeRepository = _scope.ServiceProvider.GetRequiredService<IDegreeRepository>();
            CertificationRepository = _scope.ServiceProvider.GetRequiredService<ICertificationRepository>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _tokenGenerator = _scope.ServiceProvider.GetRequiredService<ITokenGenerator>();
        }

        protected void AttachAccessToken(Guid userId, UserRole userRole = UserRole.User)
        {
            string accessToken = _tokenGenerator.GenerateAccessToken(
                new ClaimsAccessToken
                {
                    UserId = userId.ToString(),
                    Role = userRole.ToString()
                });

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        protected StringContent CreateJsonContent<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return new StringContent(
                JsonSerializer.Serialize(data, options),
                Encoding.UTF8,
                "application/json");
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, options);
        }

        protected async Task<User> CreateUser(
            Guid? id = null,
            string email = "user@example.com",
            string password = "P@ssword1"
        )
        {
            var user = new User
            {
                Id = id ?? Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await UserRepository.CreateAsync(user);
            await UserRepository.SaveChangesAsync();
            return user;
        }

        protected async Task<CV> CreateCV(Guid? userId = null, string title = "Test CV")
        {
            var cv = new CV
            {
                Title = title,
                UserId = userId ?? Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            await CVRepository.CreateAsync(cv);
            await CVRepository.SaveChangesAsync();
            return cv;
        }

        protected async Task<Contact> CreateContact(
            Guid? cvId = null,
            string email = "contact@example.com",
            string phone = "+1234567890"
        )
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                CVId = cvId ?? Guid.NewGuid(),
                Email = email,
                PhoneNumber = phone,
            };

            await ContactRepository.CreateAsync(contact);
            await ContactRepository.SaveChangesAsync();
            return contact;
        }

        protected async Task<Experience> CreateExperience(
            Guid? cvId = null,
            string jobTitle = "Software Developer",
            string organization = "Tech Company",
            string location = "San Francisco, CA",
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string description = "Worked on various projects",
            Guid? employmentTypeId = null,
            Guid? organizationId = null
        )
        {
            var experience = new Experience
            {
                Id = Guid.NewGuid(),
                CVId = cvId ?? Guid.NewGuid(),
                JobTitle = jobTitle,
                EmploymentTypeId = employmentTypeId ?? Guid.NewGuid(),
                Organization = organization,
                OrganizationId = organizationId,
                Location = location,
                StartDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                Description = description
            };

            await ExperienceRepository.CreateAsync(experience);
            await ExperienceRepository.SaveChangesAsync();
            return experience;
        }

        protected async Task<EmploymentType> CreateEmploymentType(
            string name = "Full-time",
            Guid? id = null
            )
        {
            var employmentType = new EmploymentType
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };

            await EmploymentTypeRepository.CreateAsync(employmentType);
            await EmploymentTypeRepository.SaveChangesAsync();
            return employmentType;
        }

        protected async Task<JobTitle> CreateJobTitle(
            string name = "Software Developer",
            Guid? id = null
            )
        {
            var jobTitle = new JobTitle
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };

            await JobTitleRepository.CreateAsync(jobTitle);
            await JobTitleRepository.SaveChangesAsync();
            return jobTitle;
        }

        protected async Task<Organization> CreateOrganization(
            string name = "Tech Company",
            Guid? id = null,
            string? logoUrl = null,
            string? description = null,
            OrganizationType organizationType = OrganizationType.Company
        )
        {
            var organization = new Organization
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                LogoUrl = logoUrl,
                Description = description,
                OrganizationType = organizationType
            };

            await OrganizationRepository.CreateAsync(organization);
            await OrganizationRepository.SaveChangesAsync();
            return organization;
        }

        protected async Task<Project> CreateProject(
            Guid? cvId = null,
            string title = "Test Project",
            string description = "This is a test project",
            string link = "https://example.com/project",
            DateOnly? startDate = null,
            DateOnly? endDate = null
        )
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                CVId = cvId ?? Guid.NewGuid(),
                Title = title,
                Description = description,
                Link = link,
                StartDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                EndDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            };

            await ProjectRepository.CreateAsync(entity: project);
            await ProjectRepository.SaveChangesAsync();
            return project;
        }

        protected async Task<Degree> CreateDegree(
    string name = "Bachelor of Science",
    string code = "BS",
    Guid? id = null
    )
        {
            var degree = new Degree
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Code = code
            };

            await DegreeRepository.CreateAsync(degree);
            await DegreeRepository.SaveChangesAsync();
            return degree;
        }

        protected async Task<Education> CreateEducation(
            Guid? cvId = null,
            string degreeName = "Bachelor of Science",
            string organizationName = "University of Example",
            Guid? degreeId = null,
            Guid? organizationId = null,
            string? fieldOfStudy = "Computer Science",
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? description = "Studied various CS topics.",
            decimal? gpa = 3.8m
        )
        {
            var education = new Education
            {
                Id = Guid.NewGuid(),
                CVId = cvId ?? Guid.NewGuid(),
                Degree = degreeName,
                DegreeId = degreeId,
                Organization = organizationName,
                OrganizationId = organizationId,
                FieldOfStudy = fieldOfStudy,
                StartDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-4)),
                EndDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Description = description,
                Gpa = gpa,
                CreatedAt = DateTime.UtcNow
            };

            await EducationRepository.CreateAsync(education);
            await EducationRepository.SaveChangesAsync();
            return education;
        }

        protected async Task<Certification> CreateCertification(
            Guid? cvId = null,
            string name = "AWS Certified Solutions Architect",
            string organization = "Amazon Web Services",
            Guid? organizationId = null,
            DateOnly? issuedDate = null,
            string? description = "Professional level certification for AWS architecture"
        )
        {
            var certification = new Certification
            {
                Id = Guid.NewGuid(),
                CVId = cvId ?? Guid.NewGuid(),
                Name = name,
                Organization = organization,
                OrganizationId = organizationId,
                IssuedDate = issuedDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await CertificationRepository.CreateAsync(certification);
            await CertificationRepository.SaveChangesAsync();
            return certification;
        }

        private async Task CleanTestDataAsync()
        {
            try
            {
                _dbContext.ChangeTracker.Clear();

                var experiences = await _dbContext.Experiences.ToListAsync();
                var contacts = await _dbContext.Contacts.ToListAsync();
                var cvs = await _dbContext.CVs.ToListAsync();
                var users = await _dbContext.Users.ToListAsync();
                var skills = await _dbContext.Skills.ToListAsync();
                var summaries = await _dbContext.Summaries.ToListAsync();
                var certifications = await _dbContext.Certifications.ToListAsync();
                var educations = await _dbContext.Educations.ToListAsync();
                var projects = await _dbContext.Projects.ToListAsync();
                var organizations = await _dbContext.Organizations.ToListAsync();
                var jobTitles = await _dbContext.JobTitles.ToListAsync();
                var employmentTypes = await _dbContext.EmploymentTypes.ToListAsync();
                var degrees = await _dbContext.Degrees.ToListAsync();

                if (projects.Any())
                    _dbContext.Projects.RemoveRange(projects);
                if (experiences.Any())
                    _dbContext.Experiences.RemoveRange(experiences);
                if (contacts.Any())
                    _dbContext.Contacts.RemoveRange(contacts);
                if (skills.Any())
                    _dbContext.Skills.RemoveRange(skills);
                if (summaries.Any())
                    _dbContext.Summaries.RemoveRange(summaries);
                if (certifications.Any())
                    _dbContext.Certifications.RemoveRange(certifications);
                if (educations.Any())
                    _dbContext.Educations.RemoveRange(educations);

                if (degrees.Any())
                    _dbContext.Degrees.RemoveRange(degrees);

                if (cvs.Any())
                    _dbContext.CVs.RemoveRange(cvs);

                if (organizations.Any())
                    _dbContext.Organizations.RemoveRange(organizations);
                if (jobTitles.Any())
                    _dbContext.JobTitles.RemoveRange(jobTitles);
                if (employmentTypes.Any())
                    _dbContext.EmploymentTypes.RemoveRange(employmentTypes);
                if (users.Any())
                    _dbContext.Users.RemoveRange(users);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning test data: {ex.Message}");
            }
        }

        public virtual async Task InitializeAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public virtual async Task DisposeAsync()
        {
            try
            {
                await CleanTestDataAsync();
            }
            finally
            {
                _scope.Dispose();
            }
        }
    }
}