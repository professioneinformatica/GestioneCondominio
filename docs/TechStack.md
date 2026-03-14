# Tech Stack and Architecture - GestioneCondominio

## 1. Overall Architecture

The project follows **Clean Architecture** principles, with a clear separation of concerns across concentric layers. Dependencies always point inward (from outer layers toward the domain).

```
┌─────────────────────────────────────────────┐
│              Presentation                    │
│   (Blazor WASM + ASP.NET Web API)           │
├─────────────────────────────────────────────┤
│              Infrastructure                  │
│   (EF Core, PostgreSQL, Storage, Email)      │
├─────────────────────────────────────────────┤
│              Application                     │
│   (Use Cases, DTOs, Interfaces, Validation)  │
├─────────────────────────────────────────────┤
│              Domain                          │
│   (Entities, Value Objects, Domain Events)   │
└─────────────────────────────────────────────┘
```

## 2. Project Structure

```
GestioneCondominio.sln
├── GestioneCondominio.Domain/              # Entities, Value Objects, Repository Interfaces
├── GestioneCondominio.Application/         # Use Cases, DTOs, Service Interfaces
├── GestioneCondominio.Infrastructure/      # Repository implementations, EF Core, External services
├── GestioneCondominio.ApiService/          # ASP.NET Web API, Controllers, Middleware
├── GestioneCondominio.Web/                 # Blazor WASM, Pages, Components
├── GestioneCondominio.Shared/              # Shared models between ApiService and Web (DTOs, Contracts)
├── GestioneCondominio.ServiceDefaults/     # Shared Aspire config: OpenTelemetry, Serilog
├── GestioneCondominio.AppHost/             # .NET Aspire orchestrator
├── GestioneCondominio.Migrations.Postgres/ # Dedicated EF Core migration project
├── GestioneCondominio.MigrationService/    # Auto-applies migrations at startup
├── GestioneCondominio.Domain.Tests/        # Domain layer unit tests
├── GestioneCondominio.Application.Tests/   # Application layer unit tests
├── GestioneCondominio.Infrastructure.Tests/# Infrastructure integration tests
└── GestioneCondominio.ApiService.Tests/    # API integration tests
```

### 2.1 GestioneCondominio.Domain
Innermost layer. No external dependencies.

- **Entities**: domain aggregates and entities (AdministrationFirm, Condominium, PropertyUnit, PropertyOwner, Assembly, etc.)
- **Value Objects**: immutable objects (Address, TaxCode, VatNumber, ApportionmentValues, etc.)
- **Enums**: domain enumerations (UnitType, ExpenseType, InterventionStatus, UserRole, etc.)
- **Interfaces**: repository contracts (ICondominiumRepository, IAdministrationFirmRepository, etc.)
- **Domain Events**: domain events (CondominiumTransferred, InstallmentOverdue, etc.)
- **Exceptions**: domain-specific exceptions

### 2.2 GestioneCondominio.Application
Use case orchestration. Depends only on Domain.

- **Use Cases / Services**: application logic organized by feature (CQRS with WolverineFx)
- **Commands and Queries**: message objects (plain C# records, no marker interfaces)
- **Handlers**: Wolverine message handlers (discovered by convention)
- **DTOs**: Data Transfer Objects for input/output
- **Interfaces**: contracts for infrastructure services (IEmailService, IDocumentStorage, etc.)
- **Validators**: input validation with FluentValidation
- **Mappings**: Mapperly source-generated mappers (compile-time, no reflection)

### 2.3 GestioneCondominio.Infrastructure
Concrete implementations. Depends on Application and Domain.

- **Persistence**: EF Core DbContext, entity configurations, repository implementations
- **Identity**: ASP.NET Identity configuration, external providers
- **Storage**: document storage service (local file system / cloud storage)
- **Email**: email and notification service
- **Import**: Excel parsing (ClosedXML) and PDF parsing (PdfPig) services
- **Multitenancy**: data isolation logic per administration firm

### 2.4 GestioneCondominio.ApiService
Backend entry point. Depends on Application and Shared.

- **Controllers**: REST endpoints organized by resource
- **Middleware**: error handling, logging, multitenancy
- **Filters**: authorization and validation filters
- **Configuration**: DI setup, authentication, CORS, Microsoft OpenAPI + Scalar

### 2.5 GestioneCondominio.Web
Blazor WebAssembly frontend. Communicates with the backend exclusively via HTTP API.

- **Pages**: Razor pages organized by functional area
- **Components**: reusable UI components
- **Services**: HTTP clients for API communication
- **State**: application state management (Fluxor or custom state)
- **Auth**: authentication integration with JWT tokens

### 2.6 GestioneCondominio.Shared
Shared models between ApiService and Web.

- **Contracts**: shared request/response DTOs
- **Constants**: shared constants
- **Enums**: shared enumerations for serialization

### 2.7 GestioneCondominio.ServiceDefaults
Shared Aspire configuration for all service projects.

- **OpenTelemetry**: distributed tracing and metrics
- **Serilog**: structured logging configuration
- **Service Defaults**: health checks, resilience policies

### 2.8 GestioneCondominio.AppHost
.NET Aspire orchestrator. Defines the application model and container resources.

- **Resources**: PostgreSQL container
- **Projects**: references to ApiService, Web, MigrationService
- **Configuration**: environment variables, connection strings, service discovery

### 2.9 GestioneCondominio.Migrations.Postgres
Dedicated EF Core migration project for PostgreSQL.

- **Migrations**: database migration files
- **DbContext factory**: design-time DbContext for migration generation

### 2.10 GestioneCondominio.MigrationService
Background service that auto-applies pending migrations at startup.

## 3. Technology Stack

### 3.1 Backend

| Technology | Purpose |
|---|---|
| **.NET 8+** | Runtime and framework |
| **.NET Aspire** | Cloud-native orchestration and service defaults |
| **ASP.NET Core Web API** | Backend REST API |
| **ASP.NET Core Identity** | Authentication and user management |
| **Entity Framework Core** | ORM for data access |
| **Npgsql** | EF Core provider for PostgreSQL |
| **WolverineFx** | Mediator / message bus for CQRS |
| **FluentValidation** | Input validation |
| **Mapperly** | Source-generated object mapping (Domain <-> DTO, compile-time, no reflection) |
| **Serilog** | Structured logging |
| **OpenTelemetry** | Distributed tracing and metrics |
| **Microsoft.AspNetCore.OpenApi** | Built-in OpenAPI document generation |
| **Scalar.AspNetCore** | Interactive API documentation UI |

### 3.2 Frontend

| Technology | Purpose |
|---|---|
| **Blazor WebAssembly** | SPA UI framework |
| **MudBlazor** | UI component library (Material Design) |
| **Blazored.LocalStorage** | Browser local storage |
| **Microsoft.AspNetCore.Components.Authorization** | Client-side authentication management |

### 3.3 Database

| Technology | Purpose |
|---|---|
| **PostgreSQL 16+** | Primary relational database |
| **EF Core Migrations** | Database schema management |

### 3.4 Document Import/Export

| Technology | Purpose |
|---|---|
| **ClosedXML** | Read/write Excel files (.xlsx) |
| **PdfPig** | PDF file parsing |
| **QuestPDF** | PDF file generation (financial statements, reports) |

### 3.5 Infrastructure and DevOps

| Technology | Purpose |
|---|---|
| **Docker** | Containerization |
| **.NET Aspire** | Local orchestration (ApiService + PostgreSQL + Web) |
| **Kubernetes** | Production orchestration (optional) |
| **GitHub Actions** | CI/CD pipeline |

### 3.6 Testing

| Technology | Purpose |
|---|---|
| **xUnit** | Test framework |
| **Moq** | Mocking |
| **FluentAssertions** | Readable assertions |
| **Testcontainers** | Integration tests with real PostgreSQL |
| **bUnit** | Blazor component tests |

## 4. Multitenancy

The multitenancy strategy is based on **row-level discriminator** (shared database):

- Every main entity has a `FirmId` field identifying the owning administration firm
- EF Core Global Query Filters ensure data isolation
- Condominiums shared between firms have association records with role (owner/delegate)
- Queries account for both owned and delegated condominiums

## 5. Authentication and Authorization

### Authentication Flow
1. User accesses the Blazor WASM frontend
2. Authenticates via local credentials or external provider
3. Backend issues a JWT token (access + refresh)
4. Frontend includes the token in every API request
5. Backend validates the token and applies authorization policies

### Supported External Providers
- Microsoft Account (OAuth 2.0)
- Microsoft Entra ID / Azure AD (OpenID Connect)
- Google (OAuth 2.0)
- Google Workspace (OAuth 2.0 with domain)
- Apple (Sign in with Apple)
- Facebook (OAuth 2.0)
- X / Twitter (OAuth 2.0)

### Authorization Policies
- Role-based (Super Admin, Firm Admin, Firm Operator, Property Owner)
- Resource-based (access only to condominiums belonging to the user's firm)
- Granular permissions for delegate firms

## 6. Document Management

### Storage Strategy
- Files are saved to local file system (development) or S3-compatible object storage (production)
- Document metadata is stored in the PostgreSQL database
- Each document is associated with: condominium, category, management year
- Files are organized in logical paths: `/{firmId}/{condominiumId}/{year}/{category}/{filename}`

## 7. Docker

### Dockerfile Strategy
- **ApiService**: multi-stage build (SDK for build, ASP.NET runtime for execution)
- **Web**: multi-stage build (SDK for build, Nginx for serving static files)
- **Local development**: .NET Aspire handles orchestration (PostgreSQL container auto-provisioned)
