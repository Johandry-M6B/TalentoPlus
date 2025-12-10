namespace TalentoPlus.Core.DTOS;

public class RegisterDto
{
    public string Document { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; 
    public string Phone { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Position { get; set; } = string.Empty;
    public string LevelOfStudy { get; set; } = string.Empty;
}