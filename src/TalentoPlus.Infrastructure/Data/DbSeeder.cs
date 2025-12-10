using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Data;

/// <summary>
/// Class to seed initial data into the database
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds roles and the initial admin user
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure the database is created
        await context.Database.MigrateAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed admin user
        await SeedAdminUserAsync(userManager);

        // Seed departments and job positions
        await SeedDepartmentsAndPositionsAsync(context);
    }

    /// <summary>
    /// Creates system roles
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Administrador", "Empleado", "RecursosHumanos" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
                Console.WriteLine($"✅ Role '{roleName}' created successfully");
            }
        }
    }

    /// <summary>
    /// Creates the initial admin user
    /// </summary>
    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@talentoplus.com";
        const string adminPassword = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrador del Sistema",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
                Console.WriteLine($"✅ Admin user created successfully");
                Console.WriteLine($"   📧 Email: {adminEmail}");
                Console.WriteLine($"   🔑 Password: {adminPassword}");
            }
            else
            {
                Console.WriteLine($"❌ Error creating admin user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   • {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"ℹ️  Admin user already exists: {adminEmail}");
        }
    }

    /// <summary>
    /// Creates initial departments and job positions
    /// </summary>
    private static async Task SeedDepartmentsAndPositionsAsync(ApplicationDbContext context)
    {
        if (await context.Departments.AnyAsync())
        {
            Console.WriteLine("ℹ️  Departments already exist, skipping seed.");
            return;
        }

        var departments = new List<Department>
        {
            new() { Name = "Recursos Humanos", Description = "Gestión del talento humano", IsActive = true },
            new() { Name = "Tecnología", Description = "Desarrollo y soporte TI", IsActive = true },
            new() { Name = "Marketing", Description = "Mercadeo y Publicidad", IsActive = true },
            new() { Name = "Ventas", Description = "Equipo comercial", IsActive = true },
            new() { Name = "Logística", Description = "Cadena de suministro y distribución", IsActive = true },
            new() { Name = "Finanzas", Description = "Contabilidad y finanzas", IsActive = true },
            new() { Name = "Operaciones", Description = "Operaciones generales", IsActive = true }
        };

        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {departments.Count} Departments created successfully");

        // Create associated job positions
        var positions = new List<JobPosition>();
        var random = new Random();

        // Map of positions by department
        var positionsMap = new Dictionary<string, string[]>
        {
            { "Recursos Humanos", new[] { "Analista de Selección", "Gerente de RRHH", "Especialista en Nómina", "Psicólogo Organizacional" } },
            { "Tecnología", new[] { "Desarrollador Backend", "Desarrollador Frontend", "Arquitecto de Software", "Soporte Técnico", "DevOps Engineer" } },
            { "Marketing", new[] { "Community Manager", "Diseñador Gráfico", "Analista de Marketing", "Director de Marketing" } },
            { "Ventas", new[] { "Ejecutivo de Ventas", "Director Comercial", "Asesor Comercial", "Key Account Manager" } },
            { "Logística", new[] { "Coordinador de Logística", "Jefe de Bodega", "Auxiliar de Despacho", "Conductor" } },
            { "Finanzas", new[] { "Contador", "Analista Financiero", "Tesorero", "Director Financiero" } },
            { "Operaciones", new[] { "Gerente de Operaciones", "Supervisor de Planta", "Operario", "Ingeniero de Procesos" } }
        };

        foreach (var dept in departments)
        {
            if (positionsMap.ContainsKey(dept.Name))
            {
                foreach (var title in positionsMap[dept.Name])
                {
                    positions.Add(new JobPosition
                    {
                        Title = title,
                        Description = $"Cargo de {title} en el área de {dept.Name}",
                        DepartmentId = dept.Id,
                        IsActive = true,
                        MinSalary = 1500000,
                        MaxSalary = 8000000
                    });
                }
            }
        }

        await context.JobPositions.AddRangeAsync(positions);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {positions.Count} Job Positions created successfully");
    }
}
