using TalentoPlus.Application.DTOs.Departments;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
}
