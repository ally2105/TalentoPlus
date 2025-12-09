using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad EducationLevel
/// </summary>
public class EducationLevelConfiguration : IEntityTypeConfiguration<EducationLevel>
{
    public void Configure(EntityTypeBuilder<EducationLevel> builder)
    {
        // Nombre de tabla
        builder.ToTable("EducationLevels");

        // Clave primaria
        builder.HasKey(e => e.Id);

        // Propiedades
        builder.Property(e => e.LevelType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.DegreeName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Institution)
            .HasMaxLength(200);

        builder.Property(e => e.GraduationYear)
            .HasMaxLength(4);

        builder.Property(e => e.FieldOfStudy)
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices
        builder.HasIndex(e => e.EmployeeId)
            .HasDatabaseName("IX_EducationLevels_EmployeeId");

        builder.HasIndex(e => e.LevelType)
            .HasDatabaseName("IX_EducationLevels_LevelType");

        // Relaciones
        builder.HasOne(e => e.Employee)
            .WithMany(emp => emp.EducationLevels)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
