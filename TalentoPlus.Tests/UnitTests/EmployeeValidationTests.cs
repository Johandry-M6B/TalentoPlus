using System.ComponentModel.DataAnnotations;
using TalentoPlus.Core.Entities;
using Xunit;
using Assert = Xunit.Assert;

namespace TalentoPlus.Tests.UnitTests
{
    public class EmployeeValidationTests
    {
        [Fact]
        public void Employee_ShouldBeInvalid_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var employee = new Employee(); // Missing required fields

            // Act
            var context = new ValidationContext(employee);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(employee, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
            Assert.Contains(results, r => r.MemberNames.Contains("LastName"));
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void Employee_ShouldBeValid_WhenAllFieldsAreCorrect()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocumentNumber = "123456789",
                Position = "Developer",
                Salary = 5000,
                JoinDate = DateTime.Now,
                Status = "Active",
                DepartmentId = 1
            };

            // Act
            var context = new ValidationContext(employee);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(employee, context, results, true);

            // Assert
            Assert.True(isValid);
        }
    }
}
