using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using System.Reflection;

namespace TalentoPlus.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos principal para TalentoPlus
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    /// <summary>
    /// Tabla de Empleados
    /// </summary>
    public DbSet<Employee> Employees { get; set; } = null!;

    /// <summary>
    /// Tabla de Departamentos
    /// </summary>
    public DbSet<Department> Departments { get; set; } = null!;

    /// <summary>
    /// Tabla de Cargos o Posiciones de Trabajo
    /// </summary>
    public DbSet<JobPosition> JobPositions { get; set; } = null!;

    /// <summary>
    /// Tabla de Niveles Educativos
    /// </summary>
    public DbSet<EducationLevel> EducationLevels { get; set; } = null!;

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones de FluentAPI automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configuración global para convenciones de nomenclatura PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convenciones para propiedades DateTime
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    // PostgreSQL usa 'timestamp without time zone' por defecto
                    // Si necesitas zona horaria, usa 'timestamp with time zone'
                    if (property.Name.Contains("CreatedAt") || 
                        property.Name.Contains("UpdatedAt") || 
                        property.Name.Contains("LastLogin"))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sobrescribir SaveChanges para actualizar automáticamente UpdatedAt
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Sobrescribir SaveChangesAsync para actualizar automáticamente UpdatedAt
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza automáticamente las propiedades de auditoría
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = null;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // No modificar CreatedAt
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }
    }
}
