# TalentoPlus S.A.S. - Employee Management System

## 📋 Project Description

TalentoPlus is a complete employee management system developed with **ASP.NET Core** and **PostgreSQL**. The system consists of:
- **Web Application (MVC)**: For HR administrators
- **REST API**: For employee queries with JWT authentication
- **AI Dashboard**: Natural language queries
- **PDF Generation**: Employee resumes
- **Excel Import**: Bulk employee upload

## 🏗️ Project Architecture

The project follows a **layered architecture** with **Clean Architecture** principles:

```
/TalentoPlus
│
├── TalentoPlus.Domain            # 🔵 Domain Layer
│   ├── Entities/                 # Business entities
│   ├── Enums/                    # Enumerations
│   └── Interfaces/               # Domain interfaces
│
├── TalentoPlus.Application       # 🟢 Application Layer
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Service interfaces
│   └── UseCases/                 # Use cases
│
├── TalentoPlus.Infrastructure    # 🟡 Infrastructure Layer
│   ├── Data/                     # EF Core DbContext
│   ├── Repositories/             # Repository implementation
│   ├── Services/                 # Services (PDF, Excel, Email, AI)
│   └── Migrations/               # DB Migrations
│
├── TalentoPlus.Web               # 🔴 Web Application (Admin)
│   ├── Controllers/              # MVC Controllers
│   ├── Views/                    # Razor Views
│   └── wwwroot/                  # Static files
│
└── TalentoPlus.Api               # 🟣 REST API (Employees)
    └── Controllers/              # API Controllers
```

### 📊 Layer Dependencies

```
Presentation (Web/Api) ──→ Application ──→ Domain
            ↓
     Infrastructure ──→ Application
            ↓
     Infrastructure ──→ Domain
```

**Golden Rule**: `Domain` depends on no one. It is the pure core of the business.

## 📦 Installed NuGet Packages

### TalentoPlus.Domain
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)

### TalentoPlus.Application
- No external dependencies (only Domain)

### TalentoPlus.Infrastructure
- `Microsoft.EntityFrameworkCore` (8.0.*)
- `Microsoft.EntityFrameworkCore.Design` (8.0.*)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.*)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `QuestPDF` (2025.7.4) - PDF Generation
- `ClosedXML` (0.105.0) - Excel file reading

### TalentoPlus.Web
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `Microsoft.EntityFrameworkCore.Design` (8.0.*)

### TalentoPlus.Api
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.*)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.*)
- `Microsoft.AspNetCore.OpenApi` (8.0.21)
- `Swashbuckle.AspNetCore` (6.6.2)

## 🔧 Technical Configuration

### Framework
- **.NET 8.0** (LTS)
- **ASP.NET Core 8.0**

### Database
- **PostgreSQL** (via Npgsql.EntityFrameworkCore.PostgreSQL)

### Authentication
- **ASP.NET Core Identity** (for Web administrators)
- **JWT Bearer Tokens** (for API employees)

## 🚀 Useful Commands

### Restore dependencies
```bash
dotnet restore
```

### Build solution
```bash
dotnet build
```

### Run web application
```bash
dotnet run --project src/TalentoPlus.Web/TalentoPlus.Web.csproj
```

### Run API
```bash
dotnet run --project src/TalentoPlus.Api/TalentoPlus.Api.csproj
```

### Create migration
```bash
dotnet ef migrations add InitialCreate --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

### Apply migrations
```bash
dotnet ef database update --project src/TalentoPlus.Infrastructure --startup-project src/TalentoPlus.Web
```

## 🐳 Docker Execution

The project is fully dockerized. To run it:

1.  Copy the environment variables example file:
    ```bash
    cp .env.example .env
    ```
2.  Edit the `.env` file with your real credentials (Database, Gemini API Key, JWT Secret).
3.  Run Docker Compose:
    ```bash
    docker compose up --build
    ```

To stop the services:
```bash
docker compose down
```

To view logs:
```bash
docker compose logs -f
```

This will launch:
*   **Web App**: http://localhost:5000
*   **API**: http://localhost:5001

> **Note on Emails**: The project is configured to use **Gmail** as the SMTP server. Credentials are pre-configured in the `docker-compose.yml` file for the development environment. Ensure your Google "App Password" is still valid if you experience delivery issues.

## 🧪 Running Tests

To run unit and integration tests:

```bash
dotnet test src/TalentoPlus.Tests/TalentoPlus.Tests.csproj
```

## 📞 Contact

Project developed for **TalentoPlus S.A.S.**
Modernization of the Human Resources area

---

**Last update**: Project Finalized and Ready for Deployment 🚀
