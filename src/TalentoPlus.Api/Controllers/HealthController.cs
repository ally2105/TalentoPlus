using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Api.Controllers;

/// <summary>
/// Controlador para verificar el estado de salud del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifica que la API esté en línea
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "TalentoPlus API"
        });
    }

    /// <summary>
    /// Verifica la conexión a la base de datos
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            _logger.LogInformation("🔍 Verificando conexión a la base de datos...");

            // Intentar abrir la conexión
            var canConnect = await _context.Database.CanConnectAsync();

            if (!canConnect)
            {
                _logger.LogError("❌ No se pudo conectar a la base de datos");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    message = "No se pudo conectar a la base de datos",
                    timestamp = DateTime.UtcNow
                });
            }

            // Obtener información de la base de datos
            var dbName = _context.Database.GetDbConnection().Database;
            var provider = _context.Database.ProviderName;

            // Verificar si existen las tablas
            var tables = new List<string>();
            bool tablesExist = true;

            try
            {
                var departmentCount = await _context.Departments.CountAsync();
                var employeeCount = await _context.Employees.CountAsync();
                var jobPositionCount = await _context.JobPositions.CountAsync();
                var educationLevelCount = await _context.EducationLevels.CountAsync();

                tables.Add($"Departments: {departmentCount} registros");
                tables.Add($"Employees: {employeeCount} registros");
                tables.Add($"JobPositions: {jobPositionCount} registros");
                tables.Add($"EducationLevels: {educationLevelCount} registros");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ Las tablas aún no existen: {Message}", ex.Message);
                tablesExist = false;
                tables.Add("Las tablas aún no han sido creadas. Ejecuta las migraciones.");
            }

            // Verificar migraciones pendientes
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            _logger.LogInformation("✅ Conexión a la base de datos exitosa");

            return Ok(new
            {
                status = "healthy",
                database = new
                {
                    name = dbName,
                    provider = provider,
                    canConnect = true,
                    tablesExist = tablesExist,
                    tables = tables,
                    migrations = new
                    {
                        applied = appliedMigrations.ToList(),
                        pending = pendingMigrations.ToList(),
                        total = appliedMigrations.Count()
                    }
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al verificar la base de datos: {Message}", ex.Message);

            return StatusCode(503, new
            {
                status = "unhealthy",
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Aplica las migraciones pendientes a la base de datos
    /// </summary>
    [HttpPost("database/migrate")]
    public async Task<IActionResult> MigrateDatabase()
    {
        try
        {
            _logger.LogInformation("🔄 Aplicando migraciones a la base de datos...");

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingList = pendingMigrations.ToList();

            if (pendingList.Count == 0)
            {
                _logger.LogInformation("✅ No hay migraciones pendientes");
                return Ok(new
                {
                    status = "success",
                    message = "No hay migraciones pendientes",
                    timestamp = DateTime.UtcNow
                });
            }

            await _context.Database.MigrateAsync();

            _logger.LogInformation("✅ Migraciones aplicadas exitosamente");

            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            return Ok(new
            {
                status = "success",
                message = $"Se aplicaron {pendingList.Count} migraciones exitosamente",
                migrationsApplied = pendingList,
                totalMigrations = appliedMigrations.Count(),
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar migraciones: {Message}", ex.Message);

            return StatusCode(500, new
            {
                status = "error",
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
