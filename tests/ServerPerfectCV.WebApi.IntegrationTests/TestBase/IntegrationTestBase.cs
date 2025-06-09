using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServicePerfectCV.Infrastructure.Data;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ServerPerfectCV.WebApi.IntegrationTests.TestBase
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        protected readonly HttpClient Client;
        protected readonly ApplicationDbContext DbContext;
        private readonly IServiceScope _scope;
        private bool _disposed = false;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            // Create an HTTP client
            Client = factory.CreateClient();

            // Create a scope to get the DbContext
            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Reset database before each test
            ResetDatabaseForTest();
        }

        protected void ResetDatabaseForTest()
        {
            // Clean database and setup new test data
            DbInitializer.ResetDatabase(DbContext);
        }

        protected void AuthenticateAsync(string userId)
        {
            // This is a simplified version - in a real scenario, you would generate a proper JWT token
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userId);
        }

        protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
        {
            return await Client.PostAsJsonAsync(url, data);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await Client.GetAsync(url);
        }

        protected static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean database before disposing
                    ResetDatabaseForTest();

                    // Dispose managed resources
                    _scope?.Dispose();
                    Client?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}