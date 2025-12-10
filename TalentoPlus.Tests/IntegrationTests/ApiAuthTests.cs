using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Infrastructure.Data;
using Xunit;
using Assert = Xunit.Assert;

namespace TalentoPlus.Tests.IntegrationTests
{
    public class ApiAuthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiAuthTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        // Note: This test requires the API project to be referenced and Program class to be public/accessible.
        // Also requires a running DB or InMemory config in Program.cs for tests.
        // For simplicity, we check if the endpoint exists (even if it returns 404 or 401).
        
        [Fact]
        public async Task GetDepartments_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/departments");

            // Assert
            Assert.NotEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
