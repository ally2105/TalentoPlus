# TalentoPlus S.A.S. - Sistema de Gestión de Empleados

## 📋 Descripción del Proyecto

TalentoPlus es un sistema completo de gestión de empleados desarrollado con **ASP.NET Core** y **PostgreSQL**. El sistema consta de:
- **Aplicación Web (MVC)**: Para administradores de RRHH
- **API REST**: Para consultas de empleados con autenticación JWT
- **Dashboard con IA**: Consultas en lenguaje natural
- **Generación de PDFs**: Hojas de vida de empleados
- **Importación Excel**: Carga masiva de empleados

## 🏗️ Arquitectura del Proyecto

El proyecto sigue una **arquitectura por capas** con principios de **Clean Architecture**:

```
/TalentoPlus
│
├── TalentoPlus.Domain            # 🔵 Capa de Dominio
│   ├── Entities/                 # Entidades de negocio
│   ├── Enums/                    # Enumeraciones
│   └── Interfaces/               # Interfaces de dominio
│
├── TalentoPlus.Application       # 🟢 Capa de Aplicación
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Interfaces de servicios
│   └── UseCases/                 # Casos de uso
│
├── TalentoPlus.Infrastructure    # 🟡 Capa de Infraestructura
│   ├── Data/                     # EF Core DbContext
│   ├── Repositories/             # Implementación de repositorios
│   ├── Services/                 # Servicios (PDF, Excel, Email, IA)
│   └── Migrations/               # Migraciones de BD
│
├── TalentoPlus.Web               # 🔴 Aplicación Web (Admin)
│   ├── Controllers/              # Controladores MVC
│   ├── Views/                    # Vistas Razor
│   └── wwwroot/                  # Archivos estáticos
│
└── TalentoPlus.Api               # 🟣 API REST (Empleados)
    └── Controllers/              # Controladores API
```

### 📊 Dependencias entre Capas

```
Presentation (Web/Api) ──→ Application ──→ Domain
            ↓
     Infrastructure ──→ Application
            ↓
     Infrastructure ──→ Domain
```

**Regla de Oro**: `Domain` no depende de nadie. Es el núcleo puro del negocio.

## 📦 Paquetes NuGet Instalados

### TalentoPlus.Domain
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)

### TalentoPlus.Application
- Ninguna dependencia externa (solo Domain)

### TalentoPlus.Infrastructure
- `Microsoft.EntityFrameworkCore` (8.0.*)
- `Microsoft.EntityFrameworkCore.Design` (8.0.*)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.*)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `QuestPDF` (2025.7.4) - Generación de PDFs
- `ClosedXML` (0.105.0) - Lectura de archivos Excel

### TalentoPlus.Web
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `Microsoft.EntityFrameworkCore.Design` (8.0.*)

### TalentoPlus.Api
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.*)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `Microsoft.AspNetCore.OpenApi` (8.0.21)
- `Swashbuckle.AspNetCore` (6.6.2)

## 🔧 Configuración Técnica

### Framework
- **.NET 8.0** (LTS)
- **ASP.NET Core 8.0**

### Base de Datos
- **PostgreSQL** (via Npgsql.EntityFrameworkCore.PostgreSQL)

### Autenticación
- **ASP.NET Core Identity** (para administradores en Web)
- **JWT Bearer Tokens** (para empleados en API)

## 🚀 Comandos Útiles

### Restaurar dependencias
```bash
dotnet restore
```

### Compilar solución
```bash
dotnet build
```

### Ejecutar aplicación web
```bash
dotnet run --project src/TalentoPlus.Web/TalentoPlus.Web.csproj
```

### Ejecutar API
```bash
dotnet run --project src/TalentoPlus.Api/TalentoPlus.Api.csproj
```

### Crear migración
```bash
dotnet ef migrations add InitialCreate --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### Aplicar migraciones
```bash
dotnet ef database update --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

## 📝 Estado Actual del Proyecto

### ✅ FASE 1 — Preparación
**US-01 - Crear arquitectura por capas** - ✔️ COMPLETADO

- [x] Creación de solución y proyectos
- [x] Referencias entre capas configuradas
- [x] Paquetes NuGet esenciales instalados
- [x] Compilación exitosa de la solución
- [x] Resolución de problemas de runtime Ubuntu 24.04

### ✅ FASE 2 — Dominio + Infraestructura base
**US-02 - Modelar entidades del dominio** - ✔️ COMPLETADO

#### Entidades Creadas:
- [x] `BaseEntity` - Clase base con propiedades comunes
- [x] `Employee` - Entidad principal de empleados
- [x] `Department` - Departamentos de la empresa
- [x] `JobPosition` - Cargos laborales
- [x] `EducationLevel` - Niveles educativos

#### Enumeraciones:
- [x] `EmployeeStatus` (Activo, Inactivo, Vacaciones, etc.)
- [x] `EducationLevelType` (Primaria, Secundaria, Técnico, Pregrado, etc.)

#### Value Objects:
- [x] `Email` - Validación y normalización de emails
- [x] `PhoneNumber` - Validación y normalización de teléfonos

#### Interfaces de Repositorio:
- [x] `IRepository<T>` - Repositorio genérico
- [x] `IEmployeeRepository` - Repositorio de empleados
- [x] `IDepartmentRepository` - Repositorio de departamentos

**Resultado**: 13 archivos de dominio, compilación exitosa sin warnings ✨

**US-03 - Configurar EF Core + PostgreSQL** - ✔️ COMPLETADO

#### Configuraciones de Base de Datos:
- [x] `DepartmentConfiguration` - FluentAPI para Department
- [x] `JobPositionConfiguration` - FluentAPI para JobPosition
- [x] `EmployeeConfiguration` - FluentAPI para Employee
- [x] `EducationLevelConfiguration` - FluentAPI para EducationLevel

#### DbContext y Repositorios:
- [x] `ApplicationDbContext` - Contexto principal con auditoría automática
- [x] `Repository<T>` - Repositorio genérico (9 métodos base)
- [x] `EmployeeRepository` - Repositorio especializado (14 métodos)
- [x] `DepartmentRepository` - Repositorio especializado (9 métodos)

#### Migraciones:
- [x] Migración `InitialCreate` generada
- [x] 4 tablas configuradas: Departments, JobPositions, Employees, EducationLevels
- [x] ~20 índices creados (únicos, compuestos, parciales)
- [x] Relaciones configuradas (Restrict, Cascade)

#### Configuración de Proyectos:
- [x] Web - DbContext y repositorios registrados
- [x] API - DbContext y repositorios registrados
- [x] Connection strings configurados para Clever Cloud
- [x] Retry policy para conexiones PostgreSQL

**Resultado**: 12 archivos de infraestructura, migración lista para aplicar 🚀

### 📄 Documentación:
- ✅ `README.md` - Documentación general del proyecto
- ✅ `docs/DOMAIN_MODEL.md` - Modelado detallado del dominio
- ✅ `docs/US-03-INFRASTRUCTURE.md` - Documentación completa de infraestructura
- ✅ `docs/CLEVER_CLOUD_SETUP.md` - Guía de configuración de Clever Cloud

### 🎯 Próximos Pasos:
- [x] ~~**US-03**: Configurar DbContext y Entity Framework~~ ✅ COMPLETADO
- [ ] **Aplicar migraciones** a Clever Cloud PostgreSQL
- [ ] **US-04**: Configurar ASP.NET Core Identity
- [ ] **US-05**: Crear servicios de aplicación (DTOs, casos de uso)
- [ ] **US-06**: Implementar importación de Excel
- [ ] **US-07**: Implementar generación de PDF

## 🛠️ Tecnologías Adicionales a Integrar

- **Servicio de Email**: SMTP para envío de correos
- **Inteligencia Artificial**: Gemini API (recomendado) o alternativas
- **Docker**: Containerización completa
- **Testing**: xUnit para pruebas unitarias e integración

## 📞 Contacto

Proyecto desarrollado para **TalentoPlus S.A.S.**  
Modernización del área de Recursos Humanos

---

**Última actualización**: Fase 2 - US-02 Modelado de Dominio Completado ✅

