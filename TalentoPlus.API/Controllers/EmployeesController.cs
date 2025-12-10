using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Infrastructure.Services;
using TalentoPlus.Infrastructure.Services.Interfaces;

namespace TalentoPlus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPdfService _pdfService;

        public EmployeesController(IEmployeeRepository employeeRepository, IPdfService pdfService)
        {
            _employeeRepository = employeeRepository;
            _pdfService = pdfService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) return NotFound();

            return Ok(employee);
        }

        [HttpGet("me/cv")]
        public async Task<IActionResult> DownloadMyCv()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) return NotFound();

            var pdfBytes = _pdfService.GenerateEmployeeCv(employee);
            return File(pdfBytes, "application/pdf", "my_cv.pdf");
        }

        // --- Admin Endpoints ---

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TalentoPlus.Core.DTOs.CreateEmployeeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = new TalentoPlus.Core.Entities.Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DocumentNumber = model.DocumentNumber,
                Position = model.Position,
                Salary = model.Salary,
                JoinDate = model.JoinDate,
                Status = model.Status,
                EducationLevel = model.EducationLevel,
                ProfessionalProfile = model.ProfessionalProfile,
                ContactPhone = model.ContactPhone,
                DepartmentId = model.DepartmentId
            };

            await _employeeRepository.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TalentoPlus.Core.DTOs.CreateEmployeeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Email = model.Email;
            employee.DocumentNumber = model.DocumentNumber;
            employee.Position = model.Position;
            employee.Salary = model.Salary;
            employee.JoinDate = model.JoinDate;
            employee.Status = model.Status;
            employee.EducationLevel = model.EducationLevel;
            employee.ProfessionalProfile = model.ProfessionalProfile;
            employee.ContactPhone = model.ContactPhone;
            employee.DepartmentId = model.DepartmentId;

            await _employeeRepository.UpdateAsync(employee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _employeeRepository.ExistsAsync(id)) return NotFound();
            await _employeeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
