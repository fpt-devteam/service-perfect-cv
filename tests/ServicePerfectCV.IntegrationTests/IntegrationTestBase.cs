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

namespace ServicePerfectCV.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        protected readonly HttpClient Client;
        protected readonly IUserRepository UserRepository;
        protected readonly ICVRepository CvRepository;
        protected readonly IContactRepository ContactRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _dbContext;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();
            _scope = factory.Services.CreateScope();
            UserRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
            CvRepository = _scope.ServiceProvider.GetRequiredService<ICVRepository>();
            ContactRepository = _scope.ServiceProvider.GetRequiredService<IContactRepository>();
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
            await CvRepository.CreateAsync(cv);
            await CvRepository.SaveChangesAsync();
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
                CVId = cvId,
                Email = email,
                PhoneNumber = phone,
            };

            await ContactRepository.CreateAsync(contact);
            await ContactRepository.SaveChangesAsync();
            return contact;
        }

        private async Task CleanTestDataAsync()
        {
            _dbContext.Contacts.RemoveRange(_dbContext.Contacts);
            _dbContext.CVs.RemoveRange(_dbContext.CVs);
            _dbContext.Users.RemoveRange(_dbContext.Users);
            await _dbContext.SaveChangesAsync();
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