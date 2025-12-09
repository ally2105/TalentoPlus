using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

/// <summary>
/// Interfaz de repositorio para la entidad Department
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Obtiene un departamento por su código
    /// </summary>
    Task<Department?> GetByCodeAsync(string code);

    /// <summary>
    /// Obtiene un departamento con sus empleados
    /// </summary>
    Task<Department?> GetByIdWithEmployeesAsync(int id);

    /// <summary>
    /// Obtiene un departamento con sus posiciones de trabajo
    /// </summary>
    Task<Department?> GetByIdWithJobPositionsAsync(int id);

    /// <summary>
    /// Obtiene un departamento con toda su información relacionada
    /// </summary>
    Task<Department?> GetByIdWithDetailsAsync(int id);

    /// <summary>
    /// Busca departamentos por nombre
    /// </summary>
    Task<IEnumerable<Department>> SearchByNameAsync(string name);

    /// <summary>
    /// Verifica si un código de departamento ya existe
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeDepartmentId = null);

    /// <summary>
    /// Obtiene departamentos con cantidad de empleados
    /// </summary>
    Task<IEnumerable<(Department Department, int EmployeeCount)>> GetDepartmentsWithEmployeeCountAsync();

    /// <summary>
    /// Verifica si un departamento tiene empleados asignados
    /// </summary>
    Task<bool> HasEmployeesAsync(int departmentId);

    /// <summary>
    /// Verifica si un departamento tiene posiciones de trabajo asignadas
    /// </summary>
    Task<bool> HasJobPositionsAsync(int departmentId);
}
