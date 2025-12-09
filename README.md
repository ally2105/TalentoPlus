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

## 🐳 Ejecución con Docker

El proyecto está completamente dockerizado. Para ejecutarlo:

1.  Copia el archivo de ejemplo de variables de entorno:
    ```bash
    cp .env.example .env
    ```
2.  Edita el archivo `.env` con tus credenciales reales (Base de datos, API Key de Gemini, JWT Secret).
3.  Ejecuta docker-compose:
    ```bash
    docker-compose up --build
    ```

Esto levantará:
*   **Web App**: http://localhost:5000
*   **API**: http://localhost:5001

## 🧪 Ejecución de Pruebas

Para ejecutar las pruebas unitarias y de integración:

```bash
dotnet test src/TalentoPlus.Tests/TalentoPlus.Tests.csproj
```

## 📝 Estado Actual del Proyecto

### ✅ FASE 1 — Preparación
**US-01 - Crear arquitectura por capas** - ✔️ COMPLETADO

### ✅ FASE 2 — Dominio + Infraestructura base
**US-02 - Modelar entidades del dominio** - ✔️ COMPLETADO
**US-03 - Configurar EF Core + PostgreSQL** - ✔️ COMPLETADO

### ✅ FASE 3 — Funcionalidades Core (Web)
**US-04 - Configurar Identity** - ✔️ COMPLETADO
**US-05 - CRUD Empleados** - ✔️ COMPLETADO
**US-06 - Importación Excel** - ✔️ COMPLETADO
**US-07 - Generación PDF** - ✔️ COMPLETADO

### ✅ FASE 4 — Dashboard + IA
**US-08 - Dashboard Estadísticas** - ✔️ COMPLETADO
**US-09 - Chatbot IA (Gemini)** - ✔️ COMPLETADO

### ✅ FASE 5 — API REST
**US-10 - Listar Departamentos** - ✔️ COMPLETADO
**US-11 - Registro Empleados + Email** - ✔️ COMPLETADO
**US-12 - Login JWT** - ✔️ COMPLETADO
**US-13 - Perfil Usuario** - ✔️ COMPLETADO
**US-14 - Descargar PDF (API)** - ✔️ COMPLETADO

### ✅ FASE 6 — Pruebas
**US-15 - Pruebas Unitarias** - ✔️ COMPLETADO
**US-16 - Pruebas de Integración** - ✔️ COMPLETADO

### ✅ FASE 7 — Deploy + Docker
**US-17 - Configurar Docker** - ✔️ COMPLETADO
**US-18 - Documentación Final** - ✔️ COMPLETADO

## 📞 Contacto

Proyecto desarrollado para **TalentoPlus S.A.S.**  
Modernización del área de Recursos Humanos

---

**Última actualización**: Proyecto Finalizado y Listo para Despliegue 🚀

