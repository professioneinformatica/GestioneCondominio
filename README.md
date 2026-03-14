# GestioneCondominio

A comprehensive **condominium management platform** built for Italian administration firms (_Studi di Amministrazione_). It streamlines the day-to-day operations of property administrators: from accounting and expense distribution to assembly management and owner communications.

## Key Features

- **Administration Firm Management** — multi-user firms with roles, permissions, and the ability to share or transfer condominium management between firms
- **Condominium Registry** — manage buildings, property units, staircases, building sections, and apportionment tables (_tabelle millesimali_)
- **Property Owner Management** — owner registry with a read-only portal for viewing accounting status, documents, and communications
- **Accounting** — chart of accounts, expense recording, automatic expense distribution based on apportionment tables, installment plans, and payment tracking
- **Financial Statements** — annual budgets (_preventivi_) and final statements (_consuntivi_) with PDF/Excel import and export
- **Assembly Management** — convocation, attendance and proxy tracking, quorum calculation, voting, resolutions, and minutes generation
- **Supplier & Maintenance** — supplier registry, contracts with expiry alerts, fault tickets, and intervention tracking
- **Communications & Notifications** — in-app messaging and email notifications for key events
- **Document Management** — categorized document archive with search, preview, and bulk download

## Architecture

The solution follows **Clean Architecture** principles with a CQRS approach:

```
Domain → Application → Infrastructure → Presentation (API + Web)
```

| Layer | Project | Description |
|-------|---------|-------------|
| Domain | `GestioneCondominio.Domain` | Entities, value objects, domain events, repository interfaces |
| Application | `GestioneCondominio.Application` | Use cases (CQRS via WolverineFx), DTOs, validators, mappers |
| Infrastructure | `GestioneCondominio.Infrastructure` | EF Core (PostgreSQL), Identity, document storage, email |
| API | `GestioneCondominio.ApiService` | ASP.NET Core Web API |
| Web | `GestioneCondominio.Web` | Blazor WebAssembly with MudBlazor |
| Shared | `GestioneCondominio.Shared` | Shared contracts between API and Web |
| Orchestration | `GestioneCondominio.AppHost` | .NET Aspire orchestrator |

## Tech Stack

- **.NET 10+** / **ASP.NET Core** / **.NET Aspire**
- **Blazor WebAssembly** with **MudBlazor**
- **PostgreSQL 16+** with **EF Core** and **Npgsql**
- **WolverineFx** (mediator / message bus for CQRS)
- **FluentValidation**, **Mapperly**, **Serilog**, **OpenTelemetry**
- **xUnit**, **FluentAssertions**, **Testcontainers**, **bUnit**

## Multitenancy

Row-level isolation via `FirmId` discriminator with EF Core Global Query Filters. Condominiums can be shared between firms through owner/delegate associations.

## Getting Started

### Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL via Aspire)

### Run with .NET Aspire

```bash
dotnet run --project GestioneCondominio.AppHost/GestioneCondominio.AppHost.csproj
```

This starts the API, Web frontend, and a PostgreSQL container. Migrations are applied automatically at startup.

### Build

```bash
dotnet build GestioneCondominio.sln
```

### Run Tests

```bash
dotnet test GestioneCondominio.sln
```

## License

All rights reserved.