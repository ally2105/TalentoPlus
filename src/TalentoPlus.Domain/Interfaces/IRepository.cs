using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

/// <summary>
/// Interfaz base para repositorios genéricos
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtiene todas las entidades
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene todas las entidades activas
    /// </summary>
    Task<IEnumerable<T>> GetAllActiveAsync();

    /// <summary>
    /// Obtiene una entidad por su ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Agrega una nueva entidad
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Elimina una entidad (físicamente)
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Elimina una entidad de forma lógica (soft delete)
    /// </summary>
    Task SoftDeleteAsync(int id);

    /// <summary>
    /// Verifica si existe una entidad con el ID especificado
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    Task<int> SaveChangesAsync();
}
