using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
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
        protected readonly ICompanyRepository CompanyRepository;
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
            CompanyRepository = _scope.ServiceProvider.GetRequiredService<ICompanyRepository>();
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
            Guid id = default,
            string email = "user@example.com",
            string password = "P@ssword1"
        )
        {
            var user = new User
            {
                Id = id == Guid.Empty ? Guid.NewGuid() : id,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await UserRepository.CreateAsync(user);
            await UserRepository.SaveChangesAsync();
            return user;
        }

        protected async Task<CV> CreateCV(Guid userId = default, string title = "Test CV")
        {
            var cv = new CV
            {
                Title = title,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await CVRepository.CreateAsync(cv);
            await CVRepository.SaveChangesAsync();
            return cv;
        }

        protected async Task<Contact> CreateContact(
            Guid cvId = default,
            string email = "contact@example.com",
            string phone = "+1234567890"
        )
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                CVId = cvId,
                Email = email,
                PhoneNumber = phone,
            };

            await ContactRepository.CreateAsync(contact);
            await ContactRepository.SaveChangesAsync();
            return contact;
        }

        protected async Task<Experience> CreateExperience(
            Guid cvId = default,
            string jobTitle = "Software Developer",
            string company = "Tech Company",
            string location = "San Francisco, CA",
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string description = "Worked on various projects",
            Guid? employmentTypeId = null
        )
        {
            var experience = new Experience
            {
                Id = Guid.NewGuid(),
                CVId = cvId,
                JobTitle = jobTitle,
                EmploymentTypeId = employmentTypeId ?? Guid.NewGuid(),
                Company = company,
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

        protected async Task<Company> CreateCompany(
            string name = "Tech Company",
            Guid? id = null
            )
        {
            var company = new Company
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };

            await CompanyRepository.CreateAsync(company);
            await CompanyRepository.SaveChangesAsync();
            return company;
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

                if (experiences.Any())
                    _dbContext.Experiences.RemoveRange(experiences);

                if (contacts.Any())
                    _dbContext.Contacts.RemoveRange(contacts);

                if (cvs.Any())
                    _dbContext.CVs.RemoveRange(cvs);

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