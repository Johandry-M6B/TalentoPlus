using OfficeOpenXml;
using TalentoPlus.Core.Entities;

using TalentoPlus.Infrastructure.Services.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<Employee> ParseEmployees(Stream fileStream)
        {
            var employees = new List<Employee>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var employee = new Employee
                    {
                        DocumentNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        FirstName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        LastName = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        // Column 4 (D) is BirthDate - Not in Entity
                        // Column 5 (E) is Address - Not in Entity
                        ContactPhone = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Email = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Position = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        Salary = decimal.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var salary) ? salary : 0,
                        JoinDate = DateTime.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out var joinDate) ? DateTime.SpecifyKind(joinDate, DateTimeKind.Utc) : DateTime.UtcNow,
                        Status = worksheet.Cells[row, 11].Value?.ToString() ?? "Active",
                        EducationLevel = worksheet.Cells[row, 12].Value?.ToString() ?? "",
                        ProfessionalProfile = worksheet.Cells[row, 13].Value?.ToString() ?? "",
                        // Map Department Name temporarily to Department object
                        Department = new Department { Name = worksheet.Cells[row, 14].Value?.ToString() ?? "General" }
                    };

                    employees.Add(employee);
                }
            }

            return employees;
        }
    }
}