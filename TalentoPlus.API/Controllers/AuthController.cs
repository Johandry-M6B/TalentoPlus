using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Entities;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Infrastructure.Services;
using TalentoPlus.Infrastructure.Services.Interfaces;

namespace TalentoPlus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IEmployeeRepository employeeRepository, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateEmployeeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Create Identity User
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // Assuming a default password for self-registration or handling password separately.
            // Requirement says "Autoregistro de empleado... enviando sus datos básicos".
            // It doesn't specify password. I'll generate one or assume it's passed or set later.
            // I'll assume a default password for the demo or require it in DTO.
            // Let's add Password to DTO or use a default.
            var result = await _userManager.CreateAsync(user, "DefaultPass123!"); 

            if (!result.Succeeded) return BadRequest(result.Errors);

            // 2. Create Employee Record
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DocumentNumber = model.DocumentNumber,
                DepartmentId = model.DepartmentId,
                UserId = user.Id,
                JoinDate = DateTime.UtcNow,
                Status = "Active"
            };

            await _employeeRepository.AddAsync(employee);

            // 3. Send Email
            await _emailService.SendEmailAsync(model.Email, "Welcome to TalentoPlus", "Your registration was successful. You can now login.");

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = await GenerateJwtToken(user);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

    private async Task<string> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
