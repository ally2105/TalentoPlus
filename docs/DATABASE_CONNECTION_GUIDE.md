# 🔍 Guía Completa: Verificar Conexión a Base de Datos PostgreSQL

## Estado Actual
❌ **Problema detectado**: No se puede conectar a la base de datos de Clever Cloud.

## Posibles Causas

1. **Host incorrecto**: `bxohwtxf1cbg7r0vfqot` no es un FQDN válido
2. **Firewall**: Clever Cloud puede bloquear conexiones desde IPs no autorizadas
3. **Credenciales incorrectas**
4. **Puerto incorrecto**
5. **SSL/TLS mal configurado**

---

## 📋 5 Métodos para Verificar la Conexión a PostgreSQL

### ✅ Método 1: Usando psql (Cliente PostgreSQL) - RECOMENDADO

**Instalación:**
```bash
sudo apt-get install postgresql-client
```

**Prueba de conexión:**
```bash
psql "Host=bxohwtxf1cbg7r0vfqot;Database=bxohwtxf1cbg7r0vfqot;Username=uo7bp4zw9pzss2zpeiip;Password=c4hFKa46mthVo5ywhHINPKYT6OfO4W;Port=50013;SSL Mode=Require"
```

**o en formato estándar:**
```bash
psql -h bxohwtxf1cbg7r0vfqot -p 50013 -U uo7bp4zw9pzss2zpeiip -d bxohwtxf1cbg7r0vfqot
```

**Salida esperada si funciona:**
```
Password for user uo7bp4zw9pzss2zpeiip:
SSL connection (protocol: TLSv1.3, cipher: TLS_AES_256_GCM_SHA384)
Type "help" for help.

bxohwtxf1cbg7r0vfqot=>
```

---

### ✅ Método 2: Usando el Health Check Endpoint (Ya creado)

**1. Inicia la API:**
```bash
cd /home/Coder/Vídeos/TalentoPlus/src/TalentoPlus.Api
dotnet run
```

**2. En otra terminal, verifica la conexión:**
```bash
# Verificar que la API esté corriendo
curl http://localhost:5209/api/Health

# Verificar la conexión a la base de datos
curl http://localhost:5209/api/Health/database | jq .
```

**3. Aplicar migraciones a través del endpoint:**
```bash
curl -X POST http://localhost:5209/api/Health/database/migrate | jq .
```

**Salida esperada si funciona:**
```json
{
  "status": "healthy",
  "database": {
    "name": "bxohwtxf1cbg7r0vfqot",
    "provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
    "canConnect": true,
    "tablesExist": true,
    "tables": [
      "Departments: 0 registros",
      "Employees: 0 registros"
    ],
    "migrations": {
      "applied": ["20251209192149_InitialCreate"],
      "pending": [],
      "total": 1
    }
  }
}
```

---

### ✅ Método 3: Usando EF Core CLI

**Verificar migraciones:**
```bash
cd /home/Coder/Vídeos/TalentoPlus
dotnet ef migrations list --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Api
```

**Aplicar migraciones:**
```bash
dotnet ef database update --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Api
```

**Ver SQL generado sin ejecutarlo:**
```bash
dotnet ef migrations script --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Api
```

---

### ✅ Método 4: Programa de Consola Simple

**Crear archivo de prueba:**
```bash
cd /home/Coder/Vídeos/TalentoPlus
dotnet new console -n DbConnectionTest
cd DbConnectionTest
dotnet add package Npgsql
```

**Código de prueba (Program.cs):**
```csharp
using Npgsql;

var connectionString = "Host=bxohwtxf1cbg7r0vfqot;Database=bxohwtxf1cbg7r0vfqot;Username=uo7bp4zw9pzss2zpeiip;Password=c4hFKa46mthVo5ywhHINPKYT6OfO4W;Port=50013;SSL Mode=Require;Trust Server Certificate=true";

Console.WriteLine("🔍 Probando conexión a PostgreSQL...");

try
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    
    Console.WriteLine("✅ ¡Conexión exitosa!");
    Console.WriteLine($"📊 Base de datos: {connection.Database}");
    Console.WriteLine($"🖥️  Servidor: {connection.Host}:{connection.Port}");
    Console.WriteLine($"📝 Versión PostgreSQL: {connection.ServerVersion}");
    
    // Probar una consulta
    using var cmd = new NpgsqlCommand("SELECT version();", connection);
    var version = await cmd.ExecuteScalarAsync();
    Console.WriteLine($"🔍 Versión completa: {version}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error de conexión: {ex.Message}");
    Console.WriteLine($"📋 Detalles: {ex.InnerException?.Message}");
}
```

**Ejecutar:**
```bash
dotnet run
```

---

### ✅ Método 5: Usando DBeaver o pgAdmin (GUI)

**DBeaver:**
1. Descargar de https://dbeaver.io/download/
2. New Connection → PostgreSQL
3. Ingresar los datos:
   - Host: `bxohwtxf1cbg7r0vfqot`
   - Port: `50013`
   - Database: `bxohwtxf1cbg7r0vfqot`
   - Username: `uo7bp4zw9pzss2zpeiip`
   - Password: `c4hFKa46mthVo5ywhHINPKYT6OfO4W`
   - SSL: Require
4. Test Connection

---

## 🔧 Solución de Problemas

### Problema 1: "Resource temporarily unavailable"
**Causa**: El host no puede ser resuelto por DNS

**Solución**: Verifica con Clever Cloud que el host sea correcto:
```bash
# Probar resolución DNS
nslookup bxohwtxf1cbg7r0vfqot

# Probar conectividad
telnet bxohwtxf1cbg7r0vfqot 50013
```

### Problema 2: "Connection timed out"
**Causa**: Firewall o IP no autorizada

**Solución**: 
1. Ir al panel de Clever Cloud
2. Agregar tu IP pública a la whitelist
3. Obtener tu IP pública:
```bash
curl ifconfig.me
```

### Problema 3: "password authentication failed"
**Causa**: Credenciales incorrectas

**Solución**: Verificar credenciales en el panel de Clever Cloud

### Problema 4: SSL/TLS errors
**Causa**: Configuración SSL incorrecta

**Solución**: Modificar connection string:
```csharp
// Opción 1: Confiar en el certificado
"SSL Mode=Require;Trust Server Certificate=true"

// Opción 2: Verificar certificado (más seguro)
"SSL Mode=Require;Trust Server Certificate=false"
```

---

## 📝 Checklist de Verificación

- [ ] ✅ El host es accesible (ping/telnet)
- [ ] ✅ Las credenciales son correctas
- [ ] ✅ El puerto está abierto
- [ ] ✅ La IP está en la whitelist de Clever Cloud
- [ ] ✅ SSL está configurado correctamente
- [ ] ✅ La base de datos existe
- [ ] ✅ El usuario tiene permisos

---

## 🎯 Próximos Pasos

1. **Verificar credenciales en Clever Cloud:**
   - Ir a https://console.clever-cloud.com/
   - Navegar a tu base de datos PostgreSQL
   - Verificar que los datos de conexión sean correctos

2. **Obtener el connection string correcto:**
   - Clever Cloud proporciona un connection string completo
   - Copiar exactamente como aparece en el panel

3. **Actualizar appsettings.json** con los datos correctos

4. **Probar conexión con psql** antes de usar EF Core

5. **Una vez conectado, aplicar migraciones:**
   ```bash
   dotnet ef database update
   ```

---

## 📊 Formatos de Connection String

### Formato .NET (actual):
```
Host=HOST;Database=DB;Username=USER;Password=PASS;Port=PORT;SSL Mode=Require;Trust Server Certificate=true
```

### Formato PostgreSQL estándar:
```
postgres://USER:PASS@HOST:PORT/DB?sslmode=require
```

### Formato con todos los parámetros:
```
Host=HOST;Port=PORT;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;Connection Lifetime=0;
```

---

## 🔐 Seguridad

**IMPORTANTE:** Las credenciales están hardcodeadas en `appsettings.json`.

**Recomendación para producción:**
```bash
# Usar variables de entorno
export ConnectionStrings__DefaultConnection="Host=...;Database=...;..."

# O usar User Secrets en desarrollo
dotnet user-secrets init --project src/TalentoPlus.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;..." --project src/TalentoPlus.Api
```

---

**Última actualización:** 2025-12-09
