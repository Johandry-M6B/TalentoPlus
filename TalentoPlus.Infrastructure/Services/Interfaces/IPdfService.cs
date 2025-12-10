using TalentoPlus.Core.Entities;

namespace TalentoPlus.Infrastructure.Services.Interfaces;

public interface IPdfService
{
    byte[] GenerateEmployeeCv(Employee employee);
}