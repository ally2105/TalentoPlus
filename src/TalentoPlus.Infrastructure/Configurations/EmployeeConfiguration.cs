using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Employee
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Nombre de tabla
        builder.ToTable("Employees");

        // Clave primaria
        builder.HasKey(e => e.Id);

        // Información Personal
        builder.Property(e => e.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DocumentType)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("CC");

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.SecondLastName)
            .HasMaxLength(100);

        builder.Property(e => e.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(e => e.Gender)
            .HasMaxLength(20);

        // Información de Contacto
        builder.Property(e => e.PersonalEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.CorporateEmail)
            .HasMaxLength(255);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AlternativePhoneNumber)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("Colombia");

        // Información Laboral
        builder.Property(e => e.HireDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(e => e.TerminationDate)
            .HasColumnType("date");

        builder.Property(e => e.Salary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EmployeeStatus.Activo);

        builder.Property(e => e.ProfessionalProfile)
            .HasMaxLength(2000);

        // Autenticación
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(500);

        builder.Property(e => e.LastLogin)
            .HasColumnType("timestamp with time zone");

        // Auditoría
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Propiedades computadas (no se mapean a la BD)
        builder.Ignore(e => e.FullName);
        builder.Ignore(e => e.Age);
        builder.Ignore(e => e.YearsOfService);
        builder.Ignore(e => e.IsCurrentlyActive);

        // Índices únicos
        builder.HasIndex(e => e.DocumentNumber)
            .IsUnique()
            .HasDatabaseName("IX_Employees_DocumentNumber");

        builder.HasIndex(e => e.PersonalEmail)
            .IsUnique()
            .HasDatabaseName("IX_Employees_PersonalEmail");

        builder.HasIndex(e => e.CorporateEmail)
            .IsUnique()
            .HasFilter("\"CorporateEmail\" IS NOT NULL")
            .HasDatabaseName("IX_Employees_CorporateEmail");

        // Índices de búsqueda
        builder.HasIndex(e => new { e.FirstName, e.LastName })
            .HasDatabaseName("IX_Employees_FullName");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Employees_Status");

        builder.HasIndex(e => e.DepartmentId)
            .HasDatabaseName("IX_Employees_DepartmentId");

        builder.HasIndex(e => e.JobPositionId)
            .HasDatabaseName("IX_Employees_JobPositionId");

        builder.HasIndex(e => e.HireDate)
            .HasDatabaseName("IX_Employees_HireDate");

        // Relaciones
        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JobPosition)
            .WithMany(j => j.Employees)
            .HasForeignKey(e => e.JobPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.EducationLevels)
            .WithOne(el => el.Employee)
            .HasForeignKey(el => el.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
