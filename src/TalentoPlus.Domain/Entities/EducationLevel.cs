using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

/// <summary>
/// Representa el nivel educativo alcanzado por un empleado
/// </summary>
public class EducationLevel : BaseEntity
{
    /// <summary>
    /// Tipo de nivel educativo
    /// </summary>
    public EducationLevelType LevelType { get; set; }

    /// <summary>
    /// Nombre del título o certificación obtenida
    /// </summary>
    public string DegreeName { get; set; } = string.Empty;

    /// <summary>
    /// Institución educativa donde se obtuvo el título
    /// </summary>
    public string? Institution { get; set; }

    /// <summary>
    /// Año de graduación
    /// </summary>
    public int? GraduationYear { get; set; }

    /// <summary>
    /// Área de estudio o especialización
    /// </summary>
    public string? FieldOfStudy { get; set; }

    /// <summary>
    /// ID del empleado al que pertenece este nivel educativo
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Empleado al que pertenece este nivel educativo
    /// </summary>
    public virtual Employee Employee { get; set; } = null!;
}
