using System.Text.RegularExpressions;

namespace TalentoPlus.Domain.ValueObjects;

/// <summary>
/// Value object que representa un correo electrónico válido
/// </summary>
public class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// Dirección de correo electrónico
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Constructor privado para forzar el uso del método Create
    /// </summary>
    private Email(string address)
    {
        Address = address;
    }

    /// <summary>
    /// Crea una instancia de Email validando el formato
    /// </summary>
    /// <param name="email">Dirección de correo a validar</param>
    /// <returns>Instancia de Email válida</returns>
    /// <exception cref="ArgumentException">Si el email no es válido</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El correo electrónico no puede estar vacío", nameof(email));
        }

        email = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException($"El correo electrónico '{email}' no tiene un formato válido", nameof(email));
        }

        return new Email(email);
    }

    /// <summary>
    /// Intenta crear un Email, devuelve null si es inválido
    /// </summary>
    public static Email? TryCreate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        try
        {
            return Create(email);
        }
        catch
        {
            return null;
        }
    }

    public override string ToString() => Address;

    public override bool Equals(object? obj)
    {
        if (obj is not Email other)
            return false;

        return Address.Equals(other.Address, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Address.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public static implicit operator string(Email email) => email.Address;
}
