using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

/// <summary>
/// Representa un empleado de la empresa TalentoPlus
/// </summary>
public class Employee : BaseEntity
{
    #region Información Personal

    /// <summary>
    /// Número de documento de identidad (cédula, pasaporte, etc.)
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento (CC, TI, CE, Pasaporte, etc.)
    /// </summary>
    public string DocumentType { get; set; } = "CC";

    /// <summary>
    /// Primer nombre del empleado
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Segundo nombre del empleado (opcional)
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Primer apellido del empleado
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Segundo apellido del empleado (opcional)
    /// </summary>
    public string? SecondLastName { get; set; }

    /// <summary>
    /// Nombre completo del empleado (calculado)
    /// </summary>
    public string FullName
    {
        get
        {
            var parts = new[] { FirstName, MiddleName, LastName, SecondLastName };
            return string.Join(" ", parts.Where(n => !string.IsNullOrWhiteSpace(n)));
        }
    }

    /// <summary>
    /// Fecha de nacimiento del empleado
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Edad del empleado (calculada)
    /// </summary>
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    /// <summary>
    /// Género del empleado
    /// </summary>
    public string? Gender { get; set; }

    #endregion

    #region Información de Contacto

    /// <summary>
    /// Correo electrónico personal del empleado
    /// </summary>
    public string PersonalEmail { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico corporativo del empleado
    /// </summary>
    public string? CorporateEmail { get; set; }

    /// <summary>
    /// Número de teléfono del empleado
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono alternativo (opcional)
    /// </summary>
    public string? AlternativePhoneNumber { get; set; }

    /// <summary>
    /// Dirección de residencia
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Ciudad de residencia
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// País de residencia
    /// </summary>
    public string Country { get; set; } = "Colombia";

    #endregion

    #region Información Laboral

    /// <summary>
    /// Fecha de ingreso a la empresa
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Fecha de terminación del contrato (si aplica)
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// Salario mensual del empleado
    /// </summary>
    public decimal Salary { get; set; }

    /// <summary>
    /// Estado actual del empleado
    /// </summary>
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;

    /// <summary>
    /// Perfil profesional o resumen del empleado
    /// </summary>
    public string? ProfessionalProfile { get; set; }

    /// <summary>
    /// Años de experiencia en la empresa (calculado)
    /// </summary>
    public double YearsOfService
    {
        get
        {
            var endDate = TerminationDate ?? DateTime.Today;
            return (endDate - HireDate).TotalDays / 365.25;
        }
    }

    #endregion

    #region Relaciones

    /// <summary>
    /// ID del departamento al que pertenece el empleado
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Departamento al que pertenece el empleado
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    /// <summary>
    /// ID del cargo que ocupa el empleado
    /// </summary>
    public int JobPositionId { get; set; }

    /// <summary>
    /// Cargo que ocupa el empleado
    /// </summary>
    public virtual JobPosition JobPosition { get; set; } = null!;

    /// <summary>
    /// Niveles educativos alcanzados por el empleado
    /// </summary>
    public virtual ICollection<EducationLevel> EducationLevels { get; set; } = new List<EducationLevel>();

    #endregion

    #region Información de Autenticación (para API)

    /// <summary>
    /// Hash de contraseña para autenticación en la API
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Último inicio de sesión del empleado
    /// </summary>
    public DateTime? LastLogin { get; set; }

    #endregion

    /// <summary>
    /// Verifica si el empleado está actualmente activo y trabajando
    /// </summary>
    public bool IsCurrentlyActive => Status == EmployeeStatus.Activo && IsActive && TerminationDate == null;

    /// <summary>
    /// Obtiene el nivel educativo más alto del empleado
    /// </summary>
    public EducationLevel? GetHighestEducationLevel()
    {
        return EducationLevels
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.LevelType)
            .FirstOrDefault();
    }
}
