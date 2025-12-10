using Microsoft.AspNetCore.Identity;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Checking if database is seeded...");

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Employee"))
            {
                await roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            // Seed Admin User
            var adminEmail = "admin@talentoplus.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                Console.WriteLine("Creating Admin user...");
                var user = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to create Admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure existing admin has the role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (context.Departments.Any())
            {
                Console.WriteLine("Database already has data. Skipping seeding.");
                return; // DB has been seeded
            }

            Console.WriteLine("Seeding database...");

            var departments = new List<Department>
            {
                new Department { Name = "Technology" },
                new Department { Name = "Human Resources" },
                new Department { Name = "Sales" },
                new Department { Name = "Marketing" }
            };

            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();

            var techDept = departments.First(d => d.Name == "Technology");
            var hrDept = departments.First(d => d.Name == "Human Resources");

            var employees = new List<Employee>
            {
                new Employee
                {
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@talentoplus.com",
                    DocumentNumber = "1001",
                    Position = "Senior Developer",
                    Salary = 8000,
                    JoinDate = DateTime.UtcNow.AddMonths(-24),
                    Status = "Active",
                    EducationLevel = "Master",
                    ProfessionalProfile = "Full Stack .NET Developer",
                    ContactPhone = "555-0101",
                    DepartmentId = techDept.Id
                },
                new Employee
                {
                    FirstName = "Bob",
                    LastName = "Jones",
                    Email = "bob.jones@talentoplus.com",
                    DocumentNumber = "1002",
                    Position = "HR Manager",
                    Salary = 6000,
                    JoinDate = DateTime.UtcNow.AddMonths(-12),
                    Status = "Active",
                    EducationLevel = "Bachelor",
                    ProfessionalProfile = "HR Specialist",
                    ContactPhone = "555-0102",
                    DepartmentId = hrDept.Id
                },
                new Employee
                {
                    FirstName = "Charlie",
                    LastName = "Brown",
                    Email = "charlie.brown@talentoplus.com",
                    DocumentNumber = "1003",
                    Position = "Junior Developer",
                    Salary = 4000,
                    JoinDate = DateTime.UtcNow.AddMonths(-1),
                    Status = "Vacation",
                    EducationLevel = "Bachelor",
                    ProfessionalProfile = "Backend Developer",
                    ContactPhone = "555-0103",
                    DepartmentId = techDept.Id
                }
            };

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
            Console.WriteLine("Database seeding completed successfully!");
        }
    }
}
