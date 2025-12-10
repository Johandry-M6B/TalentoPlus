using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string ProfessionalProfile { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string DocumentNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = "Active";
        public string EducationLevel { get; set; } = string.Empty;
        public string ProfessionalProfile { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        [Required]
        public int DepartmentId { get; set; }
    }
}