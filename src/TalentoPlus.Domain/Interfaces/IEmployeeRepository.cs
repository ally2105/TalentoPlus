using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Interfaces;

/// <summary>
/// Interfaz de repositorio para la entidad Employee
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// Obtiene un empleado por su número de documento
    /// </summary>
    Task<Employee?> GetByDocumentNumberAsync(string documentNumber);

    /// <summary>
    /// Obtiene un empleado por su correo electrónico
    /// </summary>
    Task<Employee?> GetByEmailAsync(string email);

    /// <summary>
    /// Obtiene empleados por departamento
    /// </summary>
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);

    /// <summary>
    /// Obtiene empleados por cargo
    /// </summary>
    Task<IEnumerable<Employee>> GetByJobPositionAsync(int jobPositionId);

    /// <summary>
    /// Obtiene empleados por estado
    /// </summary>
    Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status);

    /// <summary>
    /// Obtiene empleados activos (estado Activo y no soft-deleted)
    /// </summary>
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

    /// <summary>
    /// Obtiene empleados con información completa (incluye relaciones)
    /// </summary>
    Task<Employee?> GetByIdWithDetailsAsync(int id);

    /// <summary>
    /// Busca empleados por nombre o documento
    /// </summary>
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm);

    /// <summary>
    /// Verifica si un documento ya está registrado
    /// </summary>
    Task<bool> DocumentNumberExistsAsync(string documentNumber, int? excludeEmployeeId = null);

    /// <summary>
    /// Verifica si un email ya está registrado
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);

    /// <summary>
    /// Obtiene el conteo de empleados por departamento
    /// </summary>
    Task<Dictionary<int, int>> GetEmployeeCountByDepartmentAsync();

    /// <summary>
    /// Obtiene el conteo de empleados por estado
    /// </summary>
    Task<Dictionary<EmployeeStatus, int>> GetEmployeeCountByStatusAsync();

    /// <summary>
    /// Obtiene empleados contratados en un rango de fechas
    /// </summary>
    Task<IEnumerable<Employee>> GetHiredBetweenAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Elimina TODOS los empleados, cargos y departamentos (Solo para pruebas)
    /// </summary>
    Task DeleteAllAsync();
}
