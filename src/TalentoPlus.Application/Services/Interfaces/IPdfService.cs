using TalentoPlus.Application.DTOs.Employees;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IPdfService
{
    byte[] GenerateEmployeeResume(EmployeeDto employee);
}
