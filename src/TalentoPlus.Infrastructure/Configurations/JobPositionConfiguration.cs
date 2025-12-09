using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad JobPosition
/// </summary>
public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        // Nombre de tabla
        builder.ToTable("JobPositions");

        // Clave primaria
        builder.HasKey(j => j.Id);

        // Propiedades
        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Description)
            .HasMaxLength(1000);

        builder.Property(j => j.Level)
            .IsRequired();

        builder.Property(j => j.MinSalary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.MaxSalary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.CreatedAt)
            .IsRequired();

        builder.Property(j => j.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(j => j.Title)
            .HasDatabaseName("IX_JobPositions_Title");

        builder.HasIndex(j => j.DepartmentId)
            .HasDatabaseName("IX_JobPositions_DepartmentId");

        builder.HasIndex(j => j.Level)
            .HasDatabaseName("IX_JobPositions_Level");

        // Relaciones
        builder.HasOne(j => j.Department)
            .WithMany(d => d.JobPositions)
            .HasForeignKey(j => j.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Employees)
            .WithOne(e => e.JobPosition)
            .HasForeignKey(e => e.JobPositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
