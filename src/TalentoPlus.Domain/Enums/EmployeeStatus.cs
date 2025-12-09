namespace TalentoPlus.Domain.Enums;

/// <summary>
/// Representa los estados posibles de un empleado en el sistema
/// </summary>
public enum EmployeeStatus
{
    /// <summary>
    /// Empleado activo y trabajando
    /// </summary>
    Activo = 1,

    /// <summary>
    /// Empleado inactivo (suspendido, licencia sin goce, etc.)
    /// </summary>
    Inactivo = 2,

    /// <summary>
    /// Empleado en vacaciones
    /// </summary>
    Vacaciones = 3,

    /// <summary>
    /// Empleado en licencia médica
    /// </summary>
    LicenciaMedica = 4,

    /// <summary>
    /// Empleado retirado de la empresa
    /// </summary>
    Retirado = 5
}
