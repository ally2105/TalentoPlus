using Microsoft.AspNetCore.Identity;

namespace TalentoPlus.Domain.Entities;

/// <summary>
/// Usuario del sistema con Identity
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del usuario
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Última vez que el usuario inició sesión
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
