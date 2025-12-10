using TalentoPlus.Core.Entities;

namespace TalentoPlus.Infrastructure.Services.Interfaces;

public interface IExcelService
{
    List<Employee> ParseEmployees(Stream fileStream);
}