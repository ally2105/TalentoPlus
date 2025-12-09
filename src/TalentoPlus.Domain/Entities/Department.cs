namespace TalentoPlus.Domain.Entities;

/// <summary>
/// Representa un departamento de la empresa
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Nombre del departamento (ej: Recursos Humanos, Tecnología, Ventas)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del departamento
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Código único del departamento (ej: RRHH, TI, VEN)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Colección de empleados pertenecientes a este departamento
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    /// <summary>
    /// Colección de posiciones de trabajo disponibles en este departamento
    /// </summary>
    public virtual ICollection<JobPosition> JobPositions { get; set; } = new List<JobPosition>();
}
