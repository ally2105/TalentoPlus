using Npgsql;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🔍 Test de Conexión a PostgreSQL - TalentoPlus         ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Leer connection string de argumentos o usar default
var connectionString = args.Length > 0 
    ? args[0] 
    : "Host=bxohwtxf1cbg7r0vfqot;Database=bxohwtxf1cbg7r0vfqot;Username=uo7bp4zw9pzss2zpeiip;Password=c4hFKa46mthVo5ywhHINPKYT6OfO4W;Port=50013;SSL Mode=Require;Trust Server Certificate=true";

Console.WriteLine("📋 Connection String:");
// Ocultar password para seguridad
var safeConnectionString = connectionString.Contains("Password=") 
    ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]+", "Password=***")
    : connectionString;
Console.WriteLine($"   {safeConnectionString}");
Console.WriteLine();

Console.WriteLine("🔄 Intentando conectar...");
Console.WriteLine();

try
{
    using var connection = new NpgsqlConnection(connectionString);
    
    Console.Write("   ⏳ Abriendo conexión... ");
    await connection.OpenAsync();
    Console.WriteLine("✅");
    
    Console.WriteLine();
    Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║  ✅ ¡CONEXIÓN EXITOSA!                                    ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    
    Console.WriteLine("📊 Información del Servidor:");
    Console.WriteLine($"   🗄️  Base de datos: {connection.Database}");
    Console.WriteLine($"   🖥️  Host: {connection.Host}");
    Console.WriteLine($"   🔌 Puerto: {connection.Port}");
    Console.WriteLine($"   👤 Usuario: {connection.UserName}");
    Console.WriteLine($"   📝 Versión PostgreSQL: {connection.ServerVersion}");
    Console.WriteLine($"   🔐 SSL: {connection.SslMode}");
    Console.WriteLine();
    
    // Probar una consulta básica
    Console.WriteLine("🔍 Ejecutando consulta de prueba...");
    using var cmd = new NpgsqlCommand("SELECT version();", connection);
    var version = await cmd.ExecuteScalarAsync();
    Console.WriteLine($"   Versión completa: {version}");
    Console.WriteLine();
    
    // Verificar si existen las tablas
    Console.WriteLine("📋 Verificando tablas existentes...");
    var checkTablesCmd = new NpgsqlCommand(@"
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'public' 
        ORDER BY table_name;
    ", connection);
    
    using var reader = await checkTablesCmd.ExecuteReaderAsync();
    var tables = new List<string>();
    while (await reader.ReadAsync())
    {
        tables.Add(reader.GetString(0));
    }
    
    if (tables.Any())
    {
        Console.WriteLine($"   ✅ Se encontraron {tables.Count} tablas:");
        foreach (var table in tables)
        {
            Console.WriteLine($"      • {table}");
        }
    }
    else
    {
        Console.WriteLine("   ⚠️  No se encontraron tablas. Ejecuta las migraciones.");
    }
    
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════");
    Console.WriteLine("✅ Todas las pruebas pasaron exitosamente");
    Console.WriteLine("═══════════════════════════════════════════════════════════");
    
    Environment.Exit(0);
}
catch (NpgsqlException ex)
{
    Console.WriteLine("❌");
    Console.WriteLine();
    Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║  ❌ ERROR DE CONEXIÓN                                     ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine($"❌ Error PostgreSQL: {ex.Message}");
    Console.WriteLine();
    
    if (ex.InnerException != null)
    {
        Console.WriteLine($"📋 Detalles adicionales: {ex.InnerException.Message}");
        Console.WriteLine();
    }
    
    Console.WriteLine("🔧 Posibles soluciones:");
    Console.WriteLine();
    
    if (ex.Message.Contains("temporarily unavailable") || ex.Message.Contains("No such host"))
    {
        Console.WriteLine("   1️⃣  El hostname no puede ser resuelto:");
        Console.WriteLine("      • Verifica que el host sea correcto en Clever Cloud");
        Console.WriteLine("      • El host debe ser un FQDN completo (ej: postgresql-xxxxx.services.clever-cloud.com)");
        Console.WriteLine();
    }
    
    if (ex.Message.Contains("Connection refused") || ex.Message.Contains("timeout"))
    {
        Console.WriteLine("   2️⃣  No se puede conectar al servidor:");
        Console.WriteLine("      • Verifica que el puerto sea correcto");
        Console.WriteLine("      • Comprueba que tu IP esté en la whitelist de Clever Cloud");
        Console.WriteLine("      • Verifica el firewall local");
        Console.WriteLine();
    }
    
    if (ex.Message.Contains("password") || ex.Message.Contains("authentication"))
    {
        Console.WriteLine("   3️⃣  Problema de autenticación:");
        Console.WriteLine("      • Verifica el usuario y contraseña en Clever Cloud");
        Console.WriteLine("      • Asegúrate de no tener espacios extra en las credenciales");
        Console.WriteLine();
    }
    
    Console.WriteLine("📚 Consulta la guía completa en:");
    Console.WriteLine("   docs/DATABASE_CONNECTION_GUIDE.md");
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════");
    
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.WriteLine("❌");
    Console.WriteLine();
    Console.WriteLine($"❌ Error inesperado: {ex.Message}");
    Console.WriteLine($"📋 Tipo: {ex.GetType().Name}");
    Console.WriteLine();
    
    if (ex.InnerException != null)
    {
        Console.WriteLine($"📋 Detalles: {ex.InnerException.Message}");
    }
    
    Environment.Exit(1);
}
