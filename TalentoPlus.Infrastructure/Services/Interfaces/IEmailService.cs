namespace TalentoPlus.Infrastructure.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}