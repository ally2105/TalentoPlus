using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Application.DTOs.Dashboard;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeListDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task RegisterAsync(EmployeeRegisterRequest request);
    Task UpdateAsync(int id, EmployeeUpdateDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<EmployeeListDto>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(int id);
    Task DeleteAllAsync();
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
