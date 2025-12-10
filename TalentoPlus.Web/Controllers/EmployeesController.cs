using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Entities;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Infrastructure.Services;
using TalentoPlus.Infrastructure.Services.Interfaces;

namespace TalentoPlus.Web.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPdfService _pdfService;
        private readonly IExcelService _excelService;
        private readonly TalentoPlus.Infrastructure.Data.ApplicationDbContext _context;

        public EmployeesController(IEmployeeRepository employeeRepository, IPdfService pdfService, IExcelService excelService, TalentoPlus.Infrastructure.Data.ApplicationDbContext context)
        {
            _employeeRepository = employeeRepository;
            _pdfService = pdfService;
            _excelService = excelService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeRepository.AddAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _employeeRepository.UpdateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var employees = _excelService.ParseEmployees(stream);
                    foreach (var emp in employees)
                    {
                        // Handle Department
                        if (emp.Department != null && !string.IsNullOrEmpty(emp.Department.Name))
                        {
                            var deptName = emp.Department.Name;
                            var department = _context.Departments.FirstOrDefault(d => d.Name == deptName);
                            if (department == null)
                            {
                                department = new Department { Name = deptName };
                                _context.Departments.Add(department);
                                await _context.SaveChangesAsync();
                            }
                            emp.DepartmentId = department.Id;
                            emp.Department = null; // Clear navigation property to avoid EF issues
                        }

                        // Check if exists to update or add
                        var existing = await _employeeRepository.GetByEmailAsync(emp.Email);
                        if (existing != null)
                        {
                            // Update logic here
                            existing.FirstName = emp.FirstName;
                            existing.LastName = emp.LastName;
                            existing.DocumentNumber = emp.DocumentNumber;
                            existing.ContactPhone = emp.ContactPhone;
                            existing.Position = emp.Position;
                            existing.Salary = emp.Salary;
                            existing.JoinDate = emp.JoinDate;
                            existing.Status = emp.Status;
                            existing.EducationLevel = emp.EducationLevel;
                            existing.ProfessionalProfile = emp.ProfessionalProfile;
                            existing.DepartmentId = emp.DepartmentId;
                            
                            await _employeeRepository.UpdateAsync(existing);
                        }
                        else
                        {
                            await _employeeRepository.AddAsync(emp);
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadCv(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GenerateEmployeeCv(employee);
            return File(pdfBytes, "application/pdf", $"{employee.FirstName}_{employee.LastName}_CV.pdf");
        }
    }
}
