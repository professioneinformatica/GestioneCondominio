# Implementation Plan - GestioneCondominio

> **Legend**: `[ ]` = to do, `[x]` = done, `[-]` = skipped/not applicable
>
> Update this file as tasks are completed.

---

## Step 0 — Prerequisites

- [x] .NET 10 SDK installed
- [x] `wasm-tools` workload installed
- [-] ~~Install Aspire workload~~ — **deprecated since Aspire 13.0**; Aspire is now SDK/NuGet-based
- [x] Verify `nuget.config` points only to official NuGet.org feed

> **Note**: The Aspire workload is no longer needed. The AppHost project uses
> `<Project Sdk="Aspire.AppHost.Sdk/13.1">` directly, and ServiceDefaults is a
> regular classlib with Aspire NuGet packages.

---

## Step 1 — Solution and Project Scaffolding

### Solution

- [x] Create solution `GestioneCondominio.sln`

### Main projects

- [x] Create `GestioneCondominio.Domain` (classlib, no references)
- [x] Create `GestioneCondominio.Shared` (classlib, no references)
- [x] Create `GestioneCondominio.Application` (classlib, ref: Domain)
- [x] Create `GestioneCondominio.Infrastructure` (classlib, ref: Domain, Application)
- [x] Create `GestioneCondominio.ServiceDefaults` (classlib + Aspire NuGet packages, no project references)
- [x] Create `GestioneCondominio.ApiService` (webapi, ref: Application, Shared, Infrastructure, ServiceDefaults)
- [x] Create `GestioneCondominio.Web` (blazorwasm, ref: Shared, ServiceDefaults)
- [x] Create `GestioneCondominio.AppHost` (`Aspire.AppHost.Sdk/13.1.2`, ref: ServiceDefaults, ApiService, Web, MigrationService)
- [x] Create `GestioneCondominio.Migrations.Postgres` (classlib, ref: Infrastructure)
- [x] Create `GestioneCondominio.MigrationService` (worker, ref: Infrastructure, ServiceDefaults)

### Test projects

- [x] Create `GestioneCondominio.Domain.Tests` (xunit, ref: Domain)
- [x] Create `GestioneCondominio.Application.Tests` (xunit, ref: Application)
- [x] Create `GestioneCondominio.Infrastructure.Tests` (xunit, ref: Infrastructure)
- [x] Create `GestioneCondominio.ApiService.Tests` (xunit, ref: ApiService)

### Folder structure (with `.gitkeep`)

- [x] Domain: `Entities/`, `ValueObjects/`, `Enums/`, `Interfaces/`, `Events/`, `Exceptions/`
- [x] Application: `Features/`, `Common/Behaviors/`, `Common/Interfaces/`, `Common/Mappings/`
- [x] Infrastructure: `Persistence/`, `Persistence/Configurations/`, `Persistence/Repositories/`, `Identity/`, `Storage/`, `Email/`, `Import/`
- [x] Shared: `Contracts/`, `Constants/`, `Enums/`
- [x] ApiService: `Controllers/`, `Middleware/`, `Filters/`
- [x] Web: `Pages/`, `Components/`, `Services/`, `State/`, `Auth/`

### Verification

- [x] `dotnet build GestioneCondominio.sln` compiles with zero errors (0 warnings, 0 errors)

---

## Step 2 — Centralized Build Configuration

### `Directory.Build.props`

- [ ] Set `TargetFramework` to `net10.0`
- [ ] Enable `ImplicitUsings` and `Nullable`
- [ ] Set `AssemblyVersion`, `FileVersion`, `Version` to `0.0.0.0`
- [ ] Enable `TreatWarningsAsErrors`

### `Directory.Packages.props` (Central Package Management)

- [ ] Add `WolverineFx`, `WolverineFx.FluentValidation`
- [ ] Add `FluentValidation.DependencyInjectionExtensions`
- [ ] Add `Riok.Mapperly`
- [ ] Add `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`
- [ ] Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- [ ] Add Aspire packages: `Aspire.Hosting.PostgreSQL`, `Microsoft.Extensions.ServiceDiscovery`, `Microsoft.Extensions.Http.Resilience`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`, `OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Instrumentation.AspNetCore`, `OpenTelemetry.Instrumentation.Http`, `OpenTelemetry.Instrumentation.Runtime`
- [ ] Add `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.OpenTelemetry`
- [ ] Add `Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`
- [ ] Add `Microsoft.AspNetCore.Authentication.JwtBearer`
- [ ] Add `MudBlazor`, `Blazored.LocalStorage`
- [ ] Add `ClosedXML`, `UglyToad.PdfPig`, `QuestPDF`
- [ ] Add `xunit`, `NSubstitute`, `FluentAssertions`
- [ ] Add `Testcontainers.PostgreSql`, `bunit`, `Microsoft.AspNetCore.Mvc.Testing`

### Verification

- [ ] `dotnet restore` resolves all packages
- [ ] `dotnet build` succeeds

---

## Step 3 — ServiceDefaults (OpenTelemetry + Serilog)

- [ ] Implement `AddServiceDefaults(this IHostApplicationBuilder)` extension method
  - [ ] Configure OpenTelemetry tracing + metrics (OTLP exporter)
  - [ ] Configure Serilog (Console sink structured JSON + OpenTelemetry sink)
  - [ ] Register health checks (`/health`, `/alive`)
  - [ ] Configure HTTP resilience defaults (`Microsoft.Extensions.Http.Resilience`)
- [ ] Implement `MapDefaultEndpoints(this WebApplication)` extension method
  - [ ] Map health check endpoints

### Verification

- [ ] Project compiles; extension methods callable from ApiService, Web, MigrationService

---

## Step 4 — AppHost (Aspire Orchestrator)

- [ ] Configure `Program.cs`
  - [ ] Define PostgreSQL container resource
  - [ ] Add MigrationService with database reference
  - [ ] Add ApiService with database reference + dependency on MigrationService
  - [ ] Add Web with reference to ApiService (service discovery)

### Verification

- [ ] `dotnet run --project GestioneCondominio.AppHost` launches Aspire dashboard
- [ ] All three services visible in dashboard

---

## Step 5 — Domain Layer (Base Abstractions)

### Base entities

- [ ] `BaseEntity` — Id (Guid), CreatedAt (DateTimeOffset), UpdatedAt (DateTimeOffset), DomainEvents list
- [ ] `BaseAuditableEntity : BaseEntity` — CreatedBy (string?), UpdatedBy (string?)
- [ ] `ITenantEntity` interface — FirmId (Guid)

### Domain events

- [ ] `IDomainEvent` marker interface (plain C#, no external dependencies)
- [ ] `AddDomainEvent()` / `ClearDomainEvents()` methods on BaseEntity

### Value object base

- [ ] `ValueObject` abstract class with `GetEqualityComponents()` pattern

### Core enums

- [ ] `UserRole` (SuperAdmin, FirmAdmin, FirmOperator, PropertyOwner)
- [ ] `UnitType` (Apartment, Garage, Cellar, Shop, Office)
- [ ] `FirmAssociationType` (Owner, Delegate)

### Repository interfaces

- [ ] `IRepository<T>` generic (GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync)
- [ ] `IUnitOfWork` (SaveChangesAsync)

### Verification

- [ ] `dotnet build GestioneCondominio.Domain` succeeds with zero NuGet package references

---

## Step 6 — Infrastructure Layer (EF Core + DbContext)

### ApplicationDbContext

- [ ] Inherit from `IdentityDbContext<ApplicationUser, ApplicationRole, Guid>`
- [ ] Override `SaveChangesAsync` (audit fields, domain event collection)
- [ ] Apply `IEntityTypeConfiguration<T>` via assembly scan
- [ ] Configure global query filters for `ITenantEntity`

### Identity entities

- [ ] `ApplicationUser : IdentityUser<Guid>` — FirstName, LastName, FirmId
- [ ] `ApplicationRole : IdentityRole<Guid>`

### EF Core interceptors

- [ ] `AuditableEntityInterceptor` — populate CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
- [ ] `DomainEventCollectorInterceptor` — collect and publish domain events via Wolverine IMessageBus

### Multitenancy

- [ ] `ITenantProvider` interface (defined in Application/Common/Interfaces)
- [ ] `HttpContextTenantProvider` implementation (extracts FirmId from JWT claims)

### Generic repository

- [ ] `Repository<T> : IRepository<T>` wrapping DbSet<T>

### DI registration

- [ ] `AddInfrastructureServices(this IServiceCollection, IConfiguration)` extension method
- [ ] Register DbContext, Identity, repositories, interceptors, tenant provider

### Verification

- [ ] `dotnet build` succeeds

---

## Step 7 — Application Layer (WolverineFx + FluentValidation + Mapperly)

### WolverineFx setup

- [ ] Configure Wolverine handler discovery (convention-based, assembly scan)
- [ ] Messages as plain C# records (no marker interfaces)
- [ ] Integrate `WolverineFx.FluentValidation` middleware

### Behaviors / middleware

- [ ] FluentValidation middleware — validates messages before handler execution
- [ ] Logging middleware (Wolverine built-in)

### Application exceptions

- [ ] `ValidationException` (list of validation errors)
- [ ] `NotFoundException`
- [ ] `ForbiddenAccessException`

### Mapperly setup

- [ ] Convention: `[Mapper]` partial classes, one per feature/aggregate
- [ ] Verify source generation works at compile time

### DI registration

- [ ] `AddApplicationServices(this IHostApplicationBuilder)` extension method
- [ ] Register Wolverine, FluentValidation validators (assembly scan)

### Verification

- [ ] `dotnet build` succeeds
- [ ] Wolverine handler discovery works

---

## Step 8 — ApiService (Web API Base + Middleware)

### Program.cs

- [ ] Call `builder.AddServiceDefaults()`
- [ ] Call `builder.AddApplicationServices()` (Wolverine + FluentValidation)
- [ ] Call `builder.Services.AddInfrastructureServices(configuration)`
- [ ] Call `builder.Services.AddOpenApi()` (Microsoft built-in OpenAPI)
- [ ] Configure CORS for Blazor WASM origin
- [ ] Configure Authentication + Authorization (JWT Bearer)
- [ ] Map `app.MapOpenApi()` + `app.MapScalarApiReference()` (Scalar UI)
- [ ] Map `app.MapDefaultEndpoints()` (health checks)

### Middleware

- [ ] `ExceptionHandlingMiddleware`
  - [ ] `ValidationException` → 400 Bad Request
  - [ ] `NotFoundException` → 404 Not Found
  - [ ] `ForbiddenAccessException` → 403 Forbidden
  - [ ] Unhandled exceptions → 500 Internal Server Error
- [ ] `TenantMiddleware` — extract FirmId from JWT, set ITenantProvider

### Verification

- [ ] `dotnet run` starts the API
- [ ] Scalar API docs accessible at `/scalar/v1`
- [ ] Health check responds at `/health`

---

## Step 9 — ASP.NET Identity + JWT Authentication

### Infrastructure/Identity

- [ ] Full Identity configuration with ApplicationUser / ApplicationRole
- [ ] `ITokenService` interface (defined in Application)
- [ ] `JwtTokenService` implementation — generates access token + refresh token
- [ ] Identity seeding: create predefined roles (SuperAdmin, FirmAdmin, FirmOperator, PropertyOwner)

### Shared/Contracts/Auth

- [ ] `LoginRequest`, `LoginResponse` (access token + refresh token + expiration)
- [ ] `RegisterRequest`, `RegisterResponse`
- [ ] `RefreshTokenRequest`

### ApiService/Controllers

- [ ] `AuthController`
  - [ ] `POST /api/auth/login`
  - [ ] `POST /api/auth/register`
  - [ ] `POST /api/auth/refresh`

### Configuration

- [ ] JWT settings in `appsettings.json` (Issuer, Audience, SecretKey, expiration times)

### Verification

- [ ] Login and register work via Scalar UI
- [ ] Valid JWT returned and decodable

---

## Step 10 — Migrations.Postgres + MigrationService

### Migrations.Postgres

- [ ] `DesignTimeDbContextFactory` for `dotnet ef migrations add`
- [ ] Initial migration: `InitialCreate` (Identity tables + base schema)

### MigrationService

- [ ] `Worker` (BackgroundService) applies pending migrations on startup
- [ ] Uses `IServiceScope` to resolve ApplicationDbContext
- [ ] Logs migration progress via Serilog
- [ ] References ServiceDefaults for health checks
- [ ] Reports healthy only after migrations complete

### Verification

- [ ] AppHost → MigrationService creates DB and applies initial migration
- [ ] Identity tables visible in PostgreSQL

---

## Step 11 — Blazor WASM (Web) Base

### Program.cs

- [ ] Register MudBlazor services (`AddMudServices`)
- [ ] Register `HttpClient` with Aspire service discovery base address
- [ ] Register `Blazored.LocalStorage`
- [ ] Register custom `AuthenticationStateProvider`

### Layout

- [ ] `MainLayout.razor` with MudBlazor (`MudLayout`, `MudAppBar`, `MudDrawer`, `MudNavMenu`)
- [ ] Dark/light theme toggle

### Authentication

- [ ] `JwtAuthenticationStateProvider` — reads JWT from local storage, parses claims
- [ ] `JwtAuthorizationMessageHandler` — attaches Bearer token to HTTP requests
- [ ] `LoginPage.razor` — login form with MudBlazor components

### Pages

- [ ] `HomePage.razor` — dashboard placeholder
- [ ] `LoginPage.razor` — functional login page

### Verification

- [ ] Frontend loads with MudBlazor layout
- [ ] Login calls API, stores JWT, redirects to home
- [ ] Navigation shows authenticated user info

---

## Step 12 — First Full Vertical Slice (AdministrationFirm)

### Domain

- [ ] `AdministrationFirm` entity (Name, VatNumber, Address, Phone, Email, PecEmail, etc.)
- [ ] `IAdministrationFirmRepository : IRepository<AdministrationFirm>`

### Application

- [ ] Messages: `CreateFirmCommand`, `UpdateFirmCommand`, `GetFirmQuery`, `GetFirmListQuery`
- [ ] Handlers: `CreateFirmCommandHandler`, `GetFirmQueryHandler`, `UpdateFirmCommandHandler`
- [ ] DTOs: `FirmDto`, `CreateFirmDto`, `UpdateFirmDto`
- [ ] Validator: `CreateFirmDtoValidator`, `UpdateFirmDtoValidator`
- [ ] Mapper: `FirmMapper` (Mapperly `[Mapper]` partial class)

### Infrastructure

- [ ] `AdministrationFirmConfiguration : IEntityTypeConfiguration<AdministrationFirm>`
- [ ] `AdministrationFirmRepository : Repository<AdministrationFirm>`
- [ ] EF migration: `AddAdministrationFirm`

### Shared

- [ ] `FirmResponse`, `CreateFirmRequest`, `UpdateFirmRequest`

### ApiService

- [ ] `FirmsController` — GET list, GET by id, POST, PUT, DELETE

### Web

- [ ] `FirmsPage.razor` — list with `MudDataGrid`
- [ ] `FirmFormDialog.razor` — create/edit dialog with `MudDialog`

### Verification

- [ ] Full CRUD works from UI → API → Database
- [ ] Validation errors display correctly
- [ ] Multitenancy filter is active

---

## Step 13 — Test Projects Setup

### Domain.Tests

- [ ] Tests for base entities (equality, domain events)
- [ ] Tests for value objects
- [ ] Tests for `AdministrationFirm` business rules

### Application.Tests

- [ ] Tests for command/query handlers (mock repositories via NSubstitute)
- [ ] Tests for FluentValidation validators
- [ ] Tests for Mapperly mappers (input/output verification)

### Infrastructure.Tests

- [ ] Integration tests with Testcontainers PostgreSQL
- [ ] Tests for repository CRUD operations
- [ ] Tests for global query filters (multitenancy isolation)
- [ ] Tests for `ApplicationDbContext` audit behavior

### ApiService.Tests

- [ ] Integration tests with `WebApplicationFactory<Program>`
- [ ] Tests for FirmsController endpoints
- [ ] Tests for ExceptionHandlingMiddleware

### Verification

- [ ] `dotnet test GestioneCondominio.sln` — all tests pass

---

## Future Steps

### Feature vertical slices

- [ ] **Step 14** — Condominium + PropertyUnit entities (second vertical slice)
- [ ] **Step 15** — PropertyOwner + OwnershipPeriod
- [ ] **Step 16** — Condominium sharing and firm associations (Owner/Delegate)

### Authentication extensions

- [ ] **Step 17** — External auth providers (Google, Microsoft, Apple, Facebook, X)
- [ ] **Step 18** — Two-factor authentication (2FA)

### Document management

- [ ] **Step 19** — Document upload/download/storage (local + S3-compatible)
- [ ] **Step 20** — Document categorization and search

### Accounting

- [ ] **Step 21** — Chart of Accounts + accounting entries
- [ ] **Step 22** — Apportionment tables + expense distribution
- [ ] **Step 23** — Installments + payments
- [ ] **Step 24** — Financial statements + budgets

### Assembly management

- [ ] **Step 25** — Convocation + agenda
- [ ] **Step 26** — Attendance, proxies, voting, quorum
- [ ] **Step 27** — Minutes + resolutions

### Suppliers and maintenance

- [ ] **Step 28** — Supplier registry + contracts
- [ ] **Step 29** — Interventions + fault tickets

### Communications

- [ ] **Step 30** — Internal communications + notification board
- [ ] **Step 31** — Email notifications + notification preferences

### Dashboard and reporting

- [ ] **Step 32** — Administrator dashboard
- [ ] **Step 33** — Owner dashboard (read-only portal)
- [ ] **Step 34** — Reports (delinquency, expenses, accounting status)

### Import/Export

- [ ] **Step 35** — Excel import/export (ClosedXML)
- [ ] **Step 36** — PDF import (PdfPig) + PDF generation (QuestPDF)

### DevOps

- [ ] **Step 37** — CI/CD pipeline (GitHub Actions)
- [ ] **Step 38** — Dockerfiles (ApiService + Web)
- [ ] **Step 39** — Production deployment (Kubernetes / cloud)

---

## Step Dependency Graph

```
Step 0  (Prerequisites — DONE)
  └→ Step 1  (Solution + project scaffolding)
       └→ Step 2  (Directory.Build.props + Directory.Packages.props)
            ├→ Step 3  (ServiceDefaults: Serilog, OpenTelemetry)
            ├→ Step 5  (Domain base abstractions)
            │    └→ Step 6  (Infrastructure: EF Core, DbContext, Identity entities)
            │         ├→ Step 7  (Application: Wolverine, FluentValidation, Mapperly)
            │         │    └→ Step 8  (ApiService: middleware, OpenAPI, Scalar)
            │         │         └→ Step 9  (Identity + JWT auth)
            │         │              └→ Step 12 (First vertical slice: AdministrationFirm)
            │         └→ Step 10 (Migrations.Postgres + MigrationService)
            └→ Step 4  (AppHost: Aspire orchestrator)
                 └→ Step 11 (Blazor Web base + MudBlazor)
                      └→ Step 12 (First vertical slice: AdministrationFirm)
                           └→ Step 13 (Test projects setup)
                                └→ Steps 14+ (Feature vertical slices)
```

---

## Key Library Choices

| Concern | Library | Rationale |
|---------|---------|-----------|
| Mediator / CQRS | **WolverineFx** | Convention-based handler discovery, built-in middleware, no marker interfaces |
| Object Mapping | **Mapperly** | Source-generated at compile time, no reflection, type-safe |
| API Documentation | **Microsoft.AspNetCore.OpenApi** + **Scalar.AspNetCore** | Built-in .NET, no third-party for OpenAPI generation |
| UI Components | **MudBlazor** | Material Design, comprehensive, active community |
| Validation | **FluentValidation** | Mature, expressive, integrates with WolverineFx |
