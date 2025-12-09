using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Department
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Nombre de tabla
        builder.ToTable("Departments");

        // Clave primaria
        builder.HasKey(d => d.Id);

        // Propiedades
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(d => d.Code)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Code");

        builder.HasIndex(d => d.Name)
            .HasDatabaseName("IX_Departments_Name");

        // Relaciones
        builder.HasMany(d => d.Employees)
            .WithOne(e => e.Department)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.JobPositions)
            .WithOne(j => j.Department)
            .HasForeignKey(j => j.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
