# Key Patterns

- **Clean Architecture**: Domain → Application → Infrastructure → Presentation. Dependencies always point inward.
- **CQRS with WolverineFx**: Commands and Queries separated via Wolverine message handlers. One handler per use case. Wolverine discovers handlers by convention (no marker interfaces needed).
- **Repository pattern**: Interfaces defined in Domain, implementations in Infrastructure.
- **FluentValidation**: Every DTO and command/query must have a validator. Auto-discovered via DI.
- **Mapperly**: Domain ↔ DTO mapping via Mapperly source-generated mappers (compile-time, no reflection).
- **No logic in controllers**: Controllers only delegate to the Application layer (Wolverine IMessageBus). Return meaningful HTTP status codes.
- **Rich domain model**: Business rules live in entities and value objects, not in services.
- **Domain Events**: Cross-aggregate communication via domain events.
- **Global Query Filters**: EF Core filters for multitenancy isolation (FirmId discriminator).
- **JWT Authentication**: Access + refresh tokens. Frontend includes token in every API request.

# Domain Entities

Key business entities organized by bounded context:

- **Administration**: AdministrationFirm, FirmUser, FirmInvitation
- **Condominium**: Condominium, PropertyUnit, Staircase, BuildingSection, ApportionmentTable
- **Ownership**: PropertyOwner, OwnershipPeriod, CondominiumFirmAssociation (owner/delegate)
- **Accounting**: ChartOfAccount, AccountingEntry, ExpenseDistribution, Installment, Payment, FinancialStatement, Budget
- **Assembly**: Assembly, AgendaItem, Attendance, Proxy, Vote, Resolution, Minutes
- **Maintenance**: Supplier, Contract, Intervention, FaultTicket
- **Communications**: Communication, Notification
- **Documents**: Document, DocumentCategory

# Multitenancy

Every main entity has a `FirmId` field. EF Core Global Query Filters ensure data isolation. Condominiums shared between firms use association records with role (owner/delegate).

# NuGet Package Management

Consider using centralized package version management via `Directory.Packages.props` at the solution root. Individual `.csproj` files reference packages without version numbers.