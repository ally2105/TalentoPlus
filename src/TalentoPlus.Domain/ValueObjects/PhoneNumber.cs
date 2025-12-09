using System.Text.RegularExpressions;

namespace TalentoPlus.Domain.ValueObjects;

/// <summary>
/// Value object que representa un número telefónico válido
/// </summary>
public class PhoneNumber
{
    // Acepta formatos como: +57 300 123 4567, 300-123-4567, 3001234567, (300) 123-4567
    private static readonly Regex PhoneRegex = new(
        @"^[\+]?[(]?[0-9]{1,4}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{1,4}[-\s\.]?[0-9]{1,9}$",
        RegexOptions.Compiled
    );

    /// <summary>
    /// Número telefónico en formato normalizado
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// Número telefónico sin formato (solo dígitos)
    /// </summary>
    public string NumberOnly { get; }

    /// <summary>
    /// Constructor privado para forzar el uso del método Create
    /// </summary>
    private PhoneNumber(string number, string numberOnly)
    {
        Number = number;
        NumberOnly = numberOnly;
    }

    /// <summary>
    /// Crea una instancia de PhoneNumber validando el formato
    /// </summary>
    /// <param name="phoneNumber">Número telefónico a validar</param>
    /// <returns>Instancia de PhoneNumber válida</returns>
    /// <exception cref="ArgumentException">Si el número no es válido</exception>
    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("El número telefónico no puede estar vacío", nameof(phoneNumber));
        }

        phoneNumber = phoneNumber.Trim();

        if (!PhoneRegex.IsMatch(phoneNumber))
        {
            throw new ArgumentException($"El número telefónico '{phoneNumber}' no tiene un formato válido", nameof(phoneNumber));
        }

        // Extraer solo los dígitos para comparación
        var numberOnly = Regex.Replace(phoneNumber, @"[^\d]", "");

        if (numberOnly.Length < 7 || numberOnly.Length > 15)
        {
            throw new ArgumentException($"El número telefónico debe tener entre 7 y 15 dígitos", nameof(phoneNumber));
        }

        return new PhoneNumber(phoneNumber, numberOnly);
    }

    /// <summary>
    /// Intenta crear un PhoneNumber, devuelve null si es inválido
    /// </summary>
    public static PhoneNumber? TryCreate(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        try
        {
            return Create(phoneNumber);
        }
        catch
        {
            return null;
        }
    }

    public override string ToString() => Number;

    public override bool Equals(object? obj)
    {
        if (obj is not PhoneNumber other)
            return false;

        // Comparar usando solo los dígitos
        return NumberOnly.Equals(other.NumberOnly);
    }

    public override int GetHashCode() => NumberOnly.GetHashCode();

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Number;
}
