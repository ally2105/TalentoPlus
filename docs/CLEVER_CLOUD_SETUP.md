# 🚀 Guía de Configuración de Base de Datos PostgreSQL en Clever Cloud

## 📋 Pasos para Configurar Clever Cloud

### 1️⃣ **Crear cuenta en Clever Cloud**
1. Ve a [https://www.clever-cloud.com/](https://www.clever-cloud.com/)
2. Crea una cuenta gratuita o inicia sesión
3. Crea una nueva organización

### 2️⃣ **Crear Base de Datos PostgreSQL**
1. En el dashboard de Clever Cloud, haz clic en **"Create..."** → **"an add-on"**
2. Selecciona **PostgreSQL**
3. Elige el plan:
   - **DEV** (Plan gratuito): 256 MB RAM, 256 MB Storage
   - **S** (Plan de pago): Más recursos
4. Selecciona la región más cercana (ej: EU - Paris)
5. Dale un nombre: `talentoplus-db`
6. Haz clic en **"Create"**

### 3️⃣ **Obtener Credenciales de Conexión**

Una vez creada la base de datos, ve a la sección **"Connection string"** o **"Environment variables"**.

Encontrarás información como:

```
Host: bxxxx-postgresql.services.clever-cloud.com
Port: 5432
Database: bxxxxxxxxxxxx
Username: uxxxxxxxxxxxxx
Password: xxxxxxxxxxxxxxxxxxxx
```

**URI completa (también disponible):**
```
postgresql://user:password@host:5432/database
```

### 4️⃣ **Configurar Connection String en el Proyecto**

#### **Opción A: Usando variables individuales**

Actualiza `/src/TalentoPlus.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=bxxxx-postgresql.services.clever-cloud.com;Database=bxxxxxxxxxxxx;Username=uxxxxxxxxxxxxx;Password=xxxxxxxxxxxxxxxxxxxx;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

#### **Opción B: Usando la URI directa**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://user:password@host:5432/database?sslmode=require"
  }
}
```

⚠️ **IMPORTANTE:** Haz lo mismo para `/src/TalentoPlus.Api/appsettings.json`

### 5️⃣ **Usar Variables de Entorno (RECOMENDADO PARA PRODUCCIÓN)**

En lugar de poner las credenciales directamente en `appsettings.json`, usa variables de entorno:

#### **Linux/macOS:**
```bash
export ConnectionStrings__DefaultConnection="Host=your-host;Database=your-db;Username=your-user;Password=your-pass;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
```

#### **Windows (PowerShell):**
```powershell
$env:ConnectionStrings__DefaultConnection="Host=your-host;Database=your-db;Username=your-user;Password=your-pass;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
```

#### **Docker/Docker Compose:**
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=your-host;Database=your-db;...
```

### 6️⃣ **Aplicar Migraciones a Clever Cloud**

Ya tenemos la migración creada (`InitialCreate`). Para aplicarla:

#### **Método 1: Desde tu máquina local**

```bash
# Asegúrate de tener la connection string configurada
dotnet ef database update --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

#### **Método 2: Aplicar automáticamente al iniciar la aplicación**

Agrega esto en `Program.cs` de **TalentoPlus.Web** (después de `var app = builder.Build();`):

```csharp
// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
```

### 7️⃣ **Verificar la Conexión**

#### **Usando el CLI de Clever Cloud:**

```bash
# Instalar CLI
npm install -g clever-cloud

# Login
clever login

# Conectarse a la BD
clever link <addon-id>
clever addon show
```

#### **Usando pgAdmin o DBeaver:**

1. Descarga [pgAdmin](https://www.pgadmin.org/) o [DBeaver](https://dbeaver.io/)
2. Crea una nueva conexión con las credenciales de Clever Cloud
3. Verifica que las tablas se crearon:
   - `Departments`
   - `JobPositions`
   - `Employees`
   - `EducationLevels`

### 8️⃣ **Verificar Tablas Creadas**

Ejecuta esta consulta en tu cliente SQL favorito:

```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public'
ORDER BY table_name;
```

Deberías ver:

- ✅ `Departments`
- ✅ `EducationLevels`
- ✅ `Employees`
- ✅ `JobPositions`
- ✅ `__EFMigrationsHistory` (tabla interna de EF Core)

---

## 🔒 **Seguridad - NO Subir Credenciales a Git**

### **Agregar al `.gitignore`:**

```gitignore
# Configuraciones locales
appsettings.Development.json
appsettings.Production.json

# Variables de entorno
.env
.env.local
.env.production
```

### **Crear `appsettings.Development.json` (Git ignored):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=tu-host-clever-cloud;Database=tu-db;Username=tu-user;Password=tu-pass;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## 📊 **Comandos Útiles de EF Core**

### **Ver lista de migraciones:**
```bash
dotnet ef migrations list --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### **Crear nueva migración:**
```bash
dotnet ef migrations add NombreMigracion --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### **Aplicar migraciones:**
```bash
dotnet ef database update --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### **Revertir última migración:**
```bash
dotnet ef migrations remove --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### **Generar script SQL:**
```bash
dotnet ef migrations script --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web --output migration.sql
```

### **Ver información de la base de datos:**
```bash
dotnet ef dbcontext info --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

---

## ✅ **Checklist de Verificación**

- [ ] Cuenta de Clever Cloud creada
- [ ] Base de datos PostgreSQL creada
- [ ] Credenciales obtenidas
- [ ] Connection string actualizada en `appsettings.json` (ambos proyectos)
- [ ] Variables de entorno configuradas (opcional)
- [ ] Migración `InitialCreate` creada
- [ ] Migraciones aplicadas con `dotnet ef database update`
- [ ] Tablas verificadas en la base de datos
- [ ] Conexión exitosa desde la aplicación
- [ ] Credenciales NO committed en Git

---

## 🚨 **Solución de Problemas**

### **Error: "No connection could be made"**
✅ Verifica que el host y puerto sean correctos  
✅ Asegúrate de tener acceso a internet  
✅ Verifica que SSL Mode esté configurado

### **Error: "password authentication failed"**
✅ Verifica usuario y contraseña  
✅ Copia las credenciales exactamente como aparecen en Clever Cloud

### **Error: "database does not exist"**
✅ Verifica el nombre de la base de datos  
✅ Asegúrate de que Clever Cloud haya creado la BD correctamente

### **Error: "A network-related or instance-specific error"**
✅ Verifica tu firewall  
✅ Prueba con `Trust Server Certificate=true`

---

## 📞 **Recursos Adicionales**

- 📚 [Documentación Clever Cloud PostgreSQL](https://www.clever-cloud.com/doc/deploy/addon/postgresql/)
- 📚 [Documentación EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- 📚 [npgsql Connection String](https://www.npgsql.org/doc/connection-string-parameters.html)

---

**Última actualización**: US-03 - Configuración de EF Core + PostgreSQL  
**Autor**: TalentoPlus Development Team
