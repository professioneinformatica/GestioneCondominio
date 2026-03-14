# Development Conventions - GestioneCondominio

## Language

- **English** for all code: class names, methods, variables, comments
- **English** for all documentation: markdown files, README, inline docs

## Naming Conventions

- **PascalCase** for classes, methods, properties, constants
- **camelCase** for local variables, parameters
- **_camelCase** for private fields (with underscore prefix)
- **UPPER_SNAKE_CASE** for environment variables

### Component Naming Patterns

| Component | Pattern | Example |
|-----------|---------|---------|
| Services | `{Feature}Service` | `CondominiumService` |
| Repositories | `I{Feature}Repository` / `{Feature}Repository` | `ICondominiumRepository` |
| DTOs | `{Feature}Dto`, `Create{Feature}Dto`, `Update{Feature}Dto` | `CondominiumDto`, `CreateCondominiumDto` |
| Response DTOs | `{Feature}ResponseDto` | `CondominiumResponseDto` |
| Validators | `{Feature}DtoValidator` / `{Entity}Validator` | `CreateCondominiumDtoValidator` |
| Controllers | `{Feature}Controller` | `CondominiumsController` |
| Commands | `Create{Feature}Command` | `CreateCondominiumCommand` |
| Queries | `Get{Feature}Query`, `Get{Feature}ListQuery` | `GetCondominiumQuery` |
| Handlers | `{Command/Query}Handler` | `CreateCondominiumCommandHandler` |
| EF Configurations | `{Entity}Configuration` | `CondominiumConfiguration` |
| Blazor Pages | `{Feature}Page.razor` | `CondominiumsPage.razor` |
| Blazor Components | `{Feature}Component.razor` | `OwnerCardComponent.razor` |
| Test Classes | `{ClassUnderTest}Tests` | `CondominiumServiceTests` |
| Test Methods | `{Method}_Should{Expected}_When{Condition}` | `Create_ShouldReturnSuccess_WhenDataIsValid` |

## Coding Style

- **Curly braces always required** for ALL control flow statements (`if`, `else`, `for`, `foreach`, `while`, `using`, `try`, `catch`, `finally`), even for single statements:

  ```csharp
  // ✅ correct
  if (condition)
  {
      DoSomething();
  }

  // ❌ wrong — never omit braces
  if (condition)
      DoSomething();
  ```

## Git

### Commit Messages
- Written in **English**
- Follow **extended Conventional Commits** format with JSON metadata body
- Full specification in `.claude/rules/git-workflow.md`
- Commit types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `style`, `perf`, `ci`, `build`

### Branch Naming
- `feature/<name>` for new features
- `bugfix/<name>` for bug fixes
- `release/<version>` for release preparation
- `hotfix/<name>` for urgent production fixes
- Development branch: `develop`
- Production branch: `main`

### Version Format
- `{year}.{quarter}.{MMdd}.{HHmm}` — Example: `2026.1.0308.1430`

## Code Organization

### Controllers
- No business logic in controllers; they delegate entirely to the Application layer
- One controller per resource/aggregate
- Use meaningful HTTP status codes

### Application Layer
- Organize by feature using CQRS (Commands and Queries via WolverineFx)
- Every use case must have unit tests
- Input validation via FluentValidation validators

### Domain Layer
- Rich domain model: business rules live in entities and value objects
- No dependencies on external libraries or frameworks
- Use domain events for cross-aggregate communication

### Infrastructure Layer
- Repository implementations follow the interfaces defined in Domain
- EF Core entity configurations in separate configuration classes (`IEntityTypeConfiguration<T>`)
- One migration per logical change

## Testing

- **Unit tests** for Domain and Application layers (xUnit + Moq + FluentAssertions)
- **Integration tests** for Infrastructure layer (Testcontainers with PostgreSQL)
- **Component tests** for Blazor components (bUnit)

## Project Dependencies

Dependencies must follow Clean Architecture rules (arrows = "depends on"):
```
GestioneCondominio.Domain             → (no dependencies)
GestioneCondominio.Application        → Domain
GestioneCondominio.Infrastructure     → Domain, Application
GestioneCondominio.ApiService         → Application, Shared
GestioneCondominio.Web                → Shared
GestioneCondominio.Shared             → (no dependencies)
GestioneCondominio.ServiceDefaults    → (no dependencies, referenced by ApiService, Web, MigrationService)
GestioneCondominio.AppHost            → ApiService, Web, MigrationService, ServiceDefaults
GestioneCondominio.Migrations.Postgres → Infrastructure
GestioneCondominio.MigrationService   → Infrastructure, ServiceDefaults
```

- **Domain** has zero external dependencies — pure C# only
- **Application** depends only on Domain; defines interfaces (repositories, services) that Infrastructure implements
- **Infrastructure** depends on Domain and Application to implement their interfaces (dependency inversion via DI)
- **ApiService** references Application (to send commands/queries via Wolverine IMessageBus) and Shared (for request/response contracts). Infrastructure is wired via DI at startup, not via project reference.
- **Web** (Blazor WASM) communicates with the backend exclusively via HTTP — it only references Shared for contracts, never Application or Infrastructure
- **Shared** has no dependencies — it contains only DTOs, contracts, constants, and enums shared between ApiService and Web
- **ServiceDefaults** provides shared Aspire configuration (OpenTelemetry, Serilog, health checks) to all service projects
- **AppHost** is the .NET Aspire orchestrator — references all runnable projects to define the application model
- **Migrations.Postgres** dedicated migration project, references Infrastructure for the DbContext
- **MigrationService** background service that auto-applies migrations at startup
