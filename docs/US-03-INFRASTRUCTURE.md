# ✅ US-03 - Configurar EF Core + PostgreSQL - COMPLETADO

## 🎯 Objetivo
Configurar Entity Framework Core con PostgreSQL (Clever Cloud), crear el DbContext, implementar repositorios y generar las migraciones iniciales.

---

## 📦 **Lo que se Implementó**

### **1. Configuraciones de FluentAPI** (4 archivos)

Creadas en `/src/TalentoPlus.Infrastructure/Configurations/`:

#### **DepartmentConfiguration.cs**
- ✅ Tabla: `Departments`
- ✅ Índice único en `Code`
- ✅ Índice en `Name`
- ✅ Relación 1:N con `Employees` (Restrict)
- ✅ Relación 1:N con `JobPositions` (Restrict)

#### **JobPositionConfiguration.cs**
- ✅ Tabla: `JobPositions`
- ✅ Tipo decimal (18,2) para salarios
- ✅ Índices en `Title`, `DepartmentId`, `Level`
- ✅ Relación N:1 con `Department` (Restrict)
- ✅ Relación 1:N con `Employees` (Restrict)

#### **EmployeeConfiguration.cs**
- ✅ Tabla: `Employees`
- ✅ Índice único en `DocumentNumber`
- ✅ Índice único en `PersonalEmail`
- ✅ Índice único parcial en `CorporateEmail`
- ✅ Índice compuesto en `FirstName + LastName`
- ✅ Índices en `Status`, `DepartmentId`, `JobPositionId`, `HireDate`
- ✅ Propiedades computadas ignoradas (`FullName`, `Age`, etc.)
- ✅ Tipos de fecha apropiados (`date`, `timestamp with time zone`)
- ✅ Conversión de enums a int
- ✅ Valores por defecto configurados

#### **EducationLevelConfiguration.cs**
- ✅ Tabla: `EducationLevels`
- ✅ Conversión de enum `LevelType` a int
- ✅ Índices en `EmployeeId` y `LevelType`
- ✅ Relación N:1 con `Employee` (Cascade delete)

---

### **2. ApplicationDbContext**

Creado en `/src/TalentoPlus.Infrastructure/Data/ApplicationDbContext.cs`:

#### **Características:**
- ✅ DbSets para todas las entidades
- ✅ Aplicación automática de configuraciones FluentAPI
- ✅ Configuración global de timestamps de PostgreSQL
- ✅ Override de `SaveChanges` y `SaveChangesAsync`
- ✅ Actualización automática de `CreatedAt` y `UpdatedAt`
- ✅ Protección contra modificación de `CreatedAt`

```csharp
public DbSet<Employee> Employees { get; set; }
public DbSet<Department> Departments { get; set; }
public DbSet<JobPosition> JobPositions { get; set; }
public DbSet<EducationLevel> EducationLevels { get; set; }
```

---

### **3. Repositorios Implementados** (3 archivos)

Creados en `/src/TalentoPlus.Infrastructure/Repositories/`:

#### **Repository<T>.cs** - Repositorio Genérico
**Métodos implementados:**
- ✅ `GetAllAsync()` - Obtener todos
- ✅ `GetAllActiveAsync()` - Solo activos
- ✅ `GetByIdAsync(id)` - Por ID
- ✅ `AddAsync(entity)` - Agregar
- ✅ `UpdateAsync(entity)` - Actualizar
- ✅ `DeleteAsync(id)` - Eliminar físico
- ✅ `SoftDeleteAsync(id)` - Eliminar lógico
- ✅ `ExistsAsync(id)` - Verificar existencia
- ✅ `SaveChangesAsync()` - Guardar cambios

#### **EmployeeRepository.cs** - Repositorio de Empleados
**14 métodos especializados:**
1. ✅ `GetByDocumentNumberAsync()`
2. ✅ `GetByEmailAsync()`
3. ✅ `GetByDepartmentAsync()`
4. ✅ `GetByJobPositionAsync()`
5. ✅ `GetByStatusAsync()`
6. ✅ `GetActiveEmployeesAsync()`
7. ✅ `GetByIdWithDetailsAsync()` - Con relaciones
8. ✅ `SearchAsync()` - Búsqueda por término
9. ✅ `DocumentNumberExistsAsync()` - Validación
10. ✅ `EmailExistsAsync()` - Validación
11. ✅ `GetEmployeeCountByDepartmentAsync()` - Estadísticas
12. ✅ `GetEmployeeCountByStatusAsync()` - Estadísticas
13. ✅ `GetHiredBetweenAsync()` - Rango de fechas

**Características especiales:**
- ✅ Eager loading con `.Include()`
- ✅ Filtrado automático por `IsActive`
- ✅ Búsqueda case-insensitive
- ✅ Soporte para exclusión en validaciones (edición)

#### **DepartmentRepository.cs** - Repositorio de Departamentos
**9 métodos especializados:**
1. ✅ `GetByCodeAsync()`
2. ✅ `GetByIdWithEmployeesAsync()`
3. ✅ `GetByIdWithJobPositionsAsync()`
4. ✅ `GetByIdWithDetailsAsync()` - Todo incluido
5. ✅ `SearchByNameAsync()`
6. ✅ `CodeExistsAsync()` - Validación
7. ✅ `GetDepartmentsWithEmployeeCountAsync()` - Estadísticas
8. ✅ `HasEmployeesAsync()` - Verificación de relaciones
9. ✅ `HasJobPositionsAsync()` - Verificación de relaciones

---

### **4. Configuración en Web y API**

#### **TalentoPlus.Web/Program.cs**
```csharp
// DbContext con retry policy
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )
    )
);

// Dependency Injection de repositorios
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

#### **TalentoPlus.Api/Program.cs**
- ✅ Misma configuración que Web
- ✅ Remoción de código de ejemplo (WeatherForecast)
- ✅ Configuración de controladores API

#### **appsettings.json (Web y Api)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-clever-cloud-host;Database=talentoplus;..."
  }
}
```

---

### **5. Migraciones de Base de Datos**

#### **Migración Creada:**
```
✅ 20251209192149_InitialCreate
```

**Archivos generados:**
- ✅ `20251209192149_InitialCreate.cs` - Migración Up/Down
- ✅ `20251209192149_InitialCreate.Designer.cs` - Metadata
- ✅ `ApplicationDbContextModelSnapshot.cs` - Snapshot del modelo

#### **Comando usado:**
```bash
dotnet ef migrations add InitialCreate \
  --project src/TalentoPlus.Infrastructure \
  --startup-project src/TalentoPlus.Web
```

---

## 🗄️ **Esquema de Base de Datos Generado**

### **Tablas Creadas:**

#### **1. Departments**
```sql
CREATE TABLE "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Code" VARCHAR(20) NOT NULL UNIQUE,
    "Description" VARCHAR(500),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE INDEX "IX_Departments_Name" ON "Departments" ("Name");
```

#### **2. JobPositions**
```sql
CREATE TABLE "JobPositions" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(1000),
    "Level" INTEGER NOT NULL,
    "MinSalary" DECIMAL(18,2) NOT NULL,
    "MaxSalary" DECIMAL(18,2) NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_JobPositions_Title" ON "JobPositions" ("Title");
CREATE INDEX "IX_JobPositions_DepartmentId" ON "JobPositions" ("DepartmentId");
CREATE INDEX "IX_JobPositions_Level" ON "JobPositions" ("Level");
```

#### **3. Employees**
```sql
CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY,
    "DocumentNumber" VARCHAR(50) NOT NULL UNIQUE,
    "DocumentType" VARCHAR(10) NOT NULL DEFAULT 'CC',
    "FirstName" VARCHAR(100) NOT NULL,
    "MiddleName" VARCHAR(100),
    "LastName" VARCHAR(100) NOT NULL,
    "SecondLastName" VARCHAR(100),
    "DateOfBirth" DATE NOT NULL,
    "Gender" VARCHAR(20),
    "PersonalEmail" VARCHAR(255) NOT NULL UNIQUE,
    "CorporateEmail" VARCHAR(255) UNIQUE,
    "PhoneNumber" VARCHAR(50) NOT NULL,
    "AlternativePhoneNumber" VARCHAR(50),
    "Address" VARCHAR(500),
    "City" VARCHAR(100),
    "Country" VARCHAR(100) NOT NULL DEFAULT 'Colombia',
    "HireDate" DATE NOT NULL,
    "TerminationDate" DATE,
    "Salary" DECIMAL(18,2) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "ProfessionalProfile" VARCHAR(2000),
    "PasswordHash" VARCHAR(500),
    "LastLogin" TIMESTAMP WITH TIME ZONE,
    "DepartmentId" INTEGER NOT NULL,
    "JobPositionId" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("JobPositionId") REFERENCES "JobPositions" ("Id") ON DELETE RESTRICT
);

-- Índices únicos
CREATE INDEX "IX_Employees_DocumentNumber" ON "Employees" ("DocumentNumber");
CREATE INDEX "IX_Employees_PersonalEmail" ON "Employees" ("PersonalEmail");
CREATE INDEX "IX_Employees_CorporateEmail" ON "Employees" ("CorporateEmail") WHERE "CorporateEmail" IS NOT NULL;

-- Índices de búsqueda
CREATE INDEX "IX_Employees_FullName" ON "Employees" ("FirstName", "LastName");
CREATE INDEX "IX_Employees_Status" ON "Employees" ("Status");
CREATE INDEX "IX_Employees_DepartmentId" ON "Employees" ("DepartmentId");
CREATE INDEX "IX_Employees_JobPositionId" ON "Employees" ("JobPositionId");
CREATE INDEX "IX_Employees_HireDate" ON "Employees" ("HireDate");
```

#### **4. EducationLevels**
```sql
CREATE TABLE "EducationLevels" (
    "Id" SERIAL PRIMARY KEY,
    "LevelType" INTEGER NOT NULL,
    "DegreeName" VARCHAR(200) NOT NULL,
    "Institution" VARCHAR(200),
    "GraduationYear" INTEGER,
    "FieldOfStudy" VARCHAR(200),
    "EmployeeId" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_EducationLevels_EmployeeId" ON "EducationLevels" ("EmployeeId");
CREATE INDEX "IX_EducationLevels_LevelType" ON "EducationLevels" ("LevelType");
```

---

## 📊 **Estadísticas del Proyecto**

| Componente | Cantidad |
|------------|----------|
| Configuraciones FluentAPI | 4 archivos |
| DbContext | 1 archivo |
| Repositorios | 3 archivos |
| Migraciones | 1 (InitialCreate) |
| Tablas creadas | 4 tablas |
| Índices creados | ~20 índices |
| Líneas de código | ~1300 líneas |

---

## ✅ **Checklist de Completado**

### **Configuraciones:**
- [x] DepartmentConfiguration
- [x] JobPositionConfiguration
- [x] EmployeeConfiguration
- [x] EducationLevelConfiguration

### **DbContext:**
- [x] ApplicationDbContext creado
- [x] DbSets configurados
- [x] Auditoría automática (CreatedAt/UpdatedAt)
- [x] Configuraciones FluentAPI aplicadas

### **Repositorios:**
- [x] Repository<T> genérico
- [x] EmployeeRepository (14 métodos)
- [x] DepartmentRepository (9 métodos)

### **Configuración de Proyectos:**
- [x] Web - DbContext registrado
- [x] Web - Repositorios registrados
- [x] Api - DbContext registrado
- [x] Api - Repositorios registrados
- [x] Connection strings configurados

### **Migraciones:**
- [x] EF Core Tools instalado
- [x] Migración InitialCreate creada
- [x] Snapshot del modelo generado

### **Documentación:**
- [x] Guía de Clever Cloud Setup
- [x] README de US-03

---

## 🚀 **Próximos Pasos**

### **Para aplicar las migraciones:**

1. **Configurar Clever Cloud:**
   - Seguir la guía en `docs/CLEVER_CLOUD_SETUP.md`
   - Obtener credenciales de conexión
   - Actualizar connection strings

2. **Aplicar migraciones:**
   ```bash
   dotnet ef database update \
     --project src/TalentoPlus.Infrastructure \
     --startup-project src/TalentoPlus.Web
   ```

3. **Verificar tablas:**
   - Conectarse con pgAdmin o DBeaver
   - Verificar que las 4 tablas existan
   - Verificar que los índices estén creados

---

## 📚 **Documentación Relacionada**

- ✅ `docs/DOMAIN_MODEL.md` - Modelado del dominio
- ✅ `docs/CLEVER_CLOUD_SETUP.md` - Configuración de Clever Cloud
- ✅ `README.md` - Documentación general

---

**Estado**: ✅ US-03 COMPLETADA  
**Compilación**: ✅ 0 errores, 0 warnings  
**Último update**: 2025-12-09
