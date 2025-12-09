namespace TalentoPlus.Domain.Entities;

/// <summary>
/// Representa un cargo o posición laboral en la empresa
/// </summary>
public class JobPosition : BaseEntity
{
    /// <summary>
    /// Nombre del cargo (ej: Desarrollador Senior, Analista, Gerente)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de las responsabilidades del cargo
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nivel jerárquico (1 = más alto, números mayores = niveles más bajos)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Rango salarial mínimo para este cargo
    /// </summary>
    public decimal MinSalary { get; set; }

    /// <summary>
    /// Rango salarial máximo para este cargo
    /// </summary>
    public decimal MaxSalary { get; set; }

    /// <summary>
    /// ID del departamento al que pertenece este cargo
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Departamento al que pertenece este cargo
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    /// <summary>
    /// Empleados que ocupan este cargo
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
