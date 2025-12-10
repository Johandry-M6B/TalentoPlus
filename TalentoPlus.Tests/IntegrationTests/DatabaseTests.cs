using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;
using Xunit;
using Assert = Xunit.Assert;

namespace TalentoPlus.Tests.IntegrationTests
{
    public class DatabaseTests
    {
        [Fact]
        public async Task CanAddAndRetrieveEmployee()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_AddEmployee")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new EmployeeRepository(context);
                var employee = new Employee
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane@example.com",
                    DocumentNumber = "987654321",
                    Department = new Department { Name = "HR" }
                };

                // Act
                await repository.AddAsync(employee);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new EmployeeRepository(context);
                var savedEmployee = await repository.GetByEmailAsync("jane@example.com");

                Assert.NotNull(savedEmployee);
                Assert.Equal("Jane", savedEmployee.FirstName);
                Assert.Equal("HR", savedEmployee.Department.Name);
            }
        }
    }
}
