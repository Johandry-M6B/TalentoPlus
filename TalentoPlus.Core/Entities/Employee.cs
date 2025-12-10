using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentoPlus.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string DocumentNumber { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public DateTime JoinDate { get; set; }

        public string Status { get; set; } = "Active"; // Active, Inactive, Vacation

        public string EducationLevel { get; set; } = string.Empty;

        public string ProfessionalProfile { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? UserId { get; set; } // Link to Identity User
    }
}
