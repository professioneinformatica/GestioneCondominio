# Coding Style

- **Curly braces always required**: Always use `{` `}` for ALL control flow and block statements, even when the body is a single statement. This applies to: `if`, `else`, `else if`, `for`, `foreach`, `while`, `do`, `switch`, `using`, `lock`, `try`, `catch`, `finally`.

  ```csharp
  // ✅ correct
  if (condition)
  {
      DoSomething();
  }

  foreach (var item in items)
  {
      Process(item);
  }

  // ❌ wrong — never omit braces
  if (condition)
      DoSomething();

  foreach (var item in items)
      Process(item);
  ```

# Naming Conventions

- **Classes/Methods/Properties**: PascalCase
- **Local variables/Parameters**: camelCase
- **Private fields**: _camelCase (with underscore prefix)
- **Constants**: PascalCase
- **Environment variables**: UPPER_SNAKE_CASE
- **Services**: `{Feature}Service`
- **Repositories**: `I{Feature}Repository` (interface in Domain), implementation in Infrastructure
- **DTOs**: `{Feature}Dto`, `Create{Feature}Dto`, `Update{Feature}Dto`, `{Feature}ResponseDto`
- **Validators**: `{Feature}DtoValidator` for DTOs; `{Entity}Validator` for domain entities
- **Controllers**: `{Feature}Controller`
- **Commands/Queries**: `Create{Feature}Command`, `Get{Feature}Query`, `Get{Feature}ListQuery`
- **Handlers**: `Create{Feature}CommandHandler`, `Get{Feature}QueryHandler`
- **Mappers**: `{Feature}Mapper` (Mapperly `[Mapper]` partial class)
- **EF Configurations**: `{Entity}Configuration` implementing `IEntityTypeConfiguration<T>`
- **Blazor Pages**: `{Feature}Page.razor`
- **Blazor Components**: `{Feature}Component.razor`