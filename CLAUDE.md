# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Additional rules loaded from `.claude/rules/`:
- `coding-style.md` — coding style and naming conventions
- `patterns.md` — key architectural patterns, domain entities, multitenancy
- `git-workflow.md` — Git Flow strategy, Conventional Commits, versioning, changelog, release process
- `fluxor-feature-pattern.md` — Fluxor/Redux scoped state pattern for Blazor UI features

## Build & Run Commands

```bash
# Build the entire solution
dotnet build GestioneCondominio.sln

# Run via .NET Aspire (starts PostgreSQL container, API, Web)
dotnet run --project GestioneCondominio.AppHost/GestioneCondominio.AppHost.csproj

# Run API service only
dotnet run --project GestioneCondominio.ApiService/GestioneCondominio.ApiService.csproj

# Run Blazor Web only
dotnet run --project GestioneCondominio.Web/GestioneCondominio.Web.csproj

# Run all tests
dotnet test GestioneCondominio.sln

# Run specific test project
dotnet test GestioneCondominio.Application.Tests/GestioneCondominio.Application.Tests.csproj
```

## Database Migrations

EF Core migrations use a dedicated migration project:

```bash
# Create a new PostgreSQL migration
dotnet ef migrations add MigrationName \
  --startup-project ./GestioneCondominio.Migrations.Postgres \
  --project ./GestioneCondominio.Migrations.Postgres

# Apply pending migrations (normally handled automatically by MigrationService at startup)
dotnet ef database update \
  --startup-project ./GestioneCondominio.Migrations.Postgres \
  --project ./GestioneCondominio.Migrations.Postgres

# Generate SQL script for a migration
dotnet ef migrations script \
  --startup-project ./GestioneCondominio.Migrations.Postgres \
  --project ./GestioneCondominio.Migrations.Postgres
```

Migrations are applied automatically by `GestioneCondominio.MigrationService` at startup.

## Architecture

This is a .NET 8+ Clean Architecture solution (`GestioneCondominio.sln`) with these layers:

### Layer Dependency Flow
```
Domain → Application → Infrastructure → Presentation (ApiService / Web)
                                       ↘ ServiceDefaults (cross-cutting)
```

### Projects

| Project | Role |
|---------|------|
| **GestioneCondominio.Domain** | Entities, value objects, domain events, repository interfaces. No external dependencies. |
| **GestioneCondominio.Application** | Use cases (CQRS via WolverineFx), DTOs, FluentValidation validators, Mapperly mappers, service interfaces |
| **GestioneCondominio.Infrastructure** | EF Core DbContext (PostgreSQL + Npgsql), entity configurations, repository implementations, Identity, document storage, email service |
| **GestioneCondominio.ApiService** | ASP.NET Web API controllers, middleware (error handling, logging, multitenancy), Microsoft OpenAPI |
| **GestioneCondominio.Web** | Blazor WebAssembly with MudBlazor components, JWT authentication, state management |
| **GestioneCondominio.Shared** | Shared request/response contracts, constants, enums between ApiService and Web |
| **GestioneCondominio.ServiceDefaults** | Shared Aspire config: OpenTelemetry, Serilog, service defaults |
| **GestioneCondominio.AppHost** | .NET Aspire orchestrator (PostgreSQL container) |
| **GestioneCondominio.Migrations.Postgres** | Dedicated EF Core migration project for PostgreSQL |
| **GestioneCondominio.MigrationService** | Auto-applies migrations at startup |

## Reference Documents

- [Functional Requirements](docs/Requirements.md)
- [Tech Stack and Architecture](docs/TechStack.md)
- [Development Conventions](docs/DevelopmentConventions.md)

## Key Domain Concepts

- **Administration Firm (Studio di Amministrazione)**: primary tenant entity, groups administrators and managed condominiums
- **Condominium (Condominio)**: building/complex managed by one firm (owner) with the option to share with other firms
- **Property Unit (Unita Immobiliare)**: individual property within a condominium (apartment, garage, cellar, shop, office)
- **Property Owner (Condomino)**: owner of one or more property units, may own units across different condominiums managed by different firms
- **Owner Firm (Studio Intestatario)**: the firm with legal and accounting responsibility for the condominium
- **Delegate Firm (Studio Delegato)**: a firm that operates on a condominium on behalf of the owner firm
