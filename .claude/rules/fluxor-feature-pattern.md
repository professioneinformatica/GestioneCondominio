# Fluxor Scoped Feature Pattern

Standard for building Blazor UI features using the Redux/Fluxor scoped state pattern.
All new features **must** follow this pattern. The scoping mechanism allows multiple independent instances of the same feature to coexist (e.g. a Condominiums grid inside a Firm detail and on its own page).

---

## Architecture Overview

```
Container (Page)          → owns ScopeId, dispatches Init/Dispose automatically
  └── Child Components    → receive ScopeId as [Parameter], read Instance, dispatch actions
        └── Actions       → all extend ScopedAction(Guid ScopeId)
              └── Reducers    → pure functions, update state via state.UpdateInstance()
              └── Effects     → async side effects (API calls), dispatch result actions
```

**Unidirectional data flow:**
```
UI dispatches Action → Reducer updates State → State notifies → UI re-renders
                     → Effect runs side effect → dispatches result Action → Reducer → ...
```

---

## Library: Iqera.Net.Blazor.Fluxor

Base classes are in the `Iqera.Net.Blazor.Fluxor` project. **Never modify these directly for feature-specific logic.**

| Class | Namespace | Role |
|-------|-----------|------|
| `ScopedStateBase<TState, TInstance>` | `Abstractions` | Base for feature state — manages `Dictionary<Guid, TInstance>` |
| `ScopedAction(Guid ScopeId)` | `Abstractions` | Base for all scoped actions |
| `InitializeScopedAction<TState>` | `Abstractions` | Generic init action (keyed by state type) |
| `DisposeScopedAction<TState>` | `Abstractions` | Generic dispose action (keyed by state type) |
| `ScopedLifecycleReducer<TState, TInstance>` | `Reducers` | Generic reducer for init/dispose — requires concrete marker |
| `ScopedFluxorContainerBase<TState, TInstance>` | `Components` | Container base (2 type params, zero overrides needed) |
| `ScopedComponentBase<TState, TInstance>` | `Components` | Child component base — receives `ScopeId`, exposes `Instance` |

---

## File Structure for a New Feature

```
Features/{FeatureName}/
├── Store/
│   ├── {FeatureName}FeatureState.cs      → FeatureState + InstanceState + GridState records
│   ├── {FeatureName}LifecycleReducer.cs  → 1-line marker class
│   └── {SubArea}/                        → one subfolder per sub-area (e.g. grid, edit)
│       ├── Actions.cs                    → scoped action records
│       ├── {SubArea}Reducer.cs           → static reducer class
│       └── {SubArea}Effects.cs           → effect class (async side effects)
├── Pages/
│   ├── {FeatureName}Page.razor           → markup with @inherits container
│   └── {FeatureName}Page.razor.cs        → code-behind (container)
└── Components/
    ├── {FeatureName}List.razor(.cs)      → grid/list component
    ├── {FeatureName}Edit.razor(.cs)      → edit form component
    └── {FeatureName}Filters.razor(.cs)   → filter panel component
```

---

## Step-by-Step: Creating a New Feature

### 1. Feature State (`Store/{FeatureName}FeatureState.cs`)

Define three levels of state records:

```csharp
using Fluxor;
using Iqera.Net.Blazor.Fluxor.Abstractions;

namespace GestioneCondominio.Web.Features.{FeatureName}.Store;

// Fluxor feature state — wraps scoped instances
[FeatureState]
public record {FeatureName}FeatureState
    : ScopedStateBase<{FeatureName}FeatureState, {FeatureName}FeatureInstanceState>
{
    private {FeatureName}FeatureState() { }
}

// Per-scope instance — groups all sub-area states
public record {FeatureName}FeatureInstanceState
{
    public {FeatureName}GridState {PluralName} { get; init; } = new();
}

// Grid/sub-area state — tracks loading, items, editing, dialog
public record {FeatureName}GridState
{
    public bool Loading { get; init; }
    public IReadOnlyList<{FeatureName}Dto> Items { get; init; }
    public int TotalCount { get; init; }
    public Edit{FeatureName}Dto? Editing { get; init; }
    public bool EditDialogOpen { get; init; }
}
```

**Rules:**
- `{FeatureName}FeatureState` constructor **must be private** (Fluxor requires parameterless but state is immutable).
- `{FeatureName}FeatureInstanceState` **must have a parameterless constructor** (required by `ScopedLifecycleReducer` `new()` constraint).
- All state records use `{ get; init; }` — never mutable properties.
- Use `record` (not `record class`) for all state types to enable value equality and `with` expressions.

### 2. Lifecycle Reducer (`Store/{FeatureName}LifecycleReducer.cs`)

A single-line marker class that enables Fluxor discovery of the generic lifecycle reducer:

```csharp
using Iqera.Net.Blazor.Fluxor.Reducers;

namespace GestioneCondominio.Web.Features.{FeatureName}.Store;

public class {FeatureName}LifecycleReducer
    : ScopedLifecycleReducer<{FeatureName}FeatureState, {FeatureName}FeatureInstanceState>;
```

**Do NOT** create per-feature `Initialize*ScopeAction` / `Dispose*ScopeAction` records. The generic `InitializeScopedAction<TState>` and `DisposeScopedAction<TState>` handle this automatically.

### 3. Actions (`Store/{SubArea}/Actions.cs`)

All actions **must** extend `ScopedAction` and include `ScopeId` as the first parameter:

```csharp
using Iqera.Net.Blazor.Fluxor.Abstractions;

namespace GestioneCondominio.Web.Features.{FeatureName}.Store.{SubArea};

// CRUD lifecycle
public record Load{PluralName}Action(Guid ScopeId, {Filter} Filter) : ScopedAction(ScopeId);
public record Load{PluralName}ResultAction(Guid ScopeId, IReadOnlyList<{Dto}> Items, int TotalCount) : ScopedAction(ScopeId);
public record Edit{Name}RequestedAction(Guid ScopeId, {Dto} Item) : ScopedAction(ScopeId);
public record Edit{Name}ProvidedAction(Guid ScopeId, Edit{Name}Dto Item) : ScopedAction(ScopeId);
public record Save{Name}RequestedAction(Guid ScopeId, Edit{Name}Dto Item) : ScopedAction(ScopeId);
public record Save{Name}SuccessAction(Guid ScopeId, {Dto} Item) : ScopedAction(ScopeId);
public record Save{Name}FailedAction(Guid ScopeId, string Error) : ScopedAction(ScopeId);
public record Close{Name}EditDialogAction(Guid ScopeId) : ScopedAction(ScopeId);
```

**Rules:**
- Always use `record` (immutable).
- Always include `Guid ScopeId` as the first parameter and pass it to `ScopedAction(ScopeId)`.
- Use consistent naming: `Load*`, `Load*Result`, `Edit*Requested`, `Edit*Provided`, `Save*Requested`, `Save*Success`, `Save*Failed`, `Close*EditDialog`.

### 4. Reducers (`Store/{SubArea}/{SubArea}Reducer.cs`)

Static class with `[ReducerMethod]` methods. Always use `state.UpdateInstance()`:

```csharp
using Fluxor;

namespace GestioneCondominio.Web.Features.{FeatureName}.Store.{SubArea};

public static class {SubArea}Reducers
{
    [ReducerMethod]
    public static {FeatureName}FeatureState ReduceLoad(
        {FeatureName}FeatureState state,
        Load{PluralName}Action action)
        => state.UpdateInstance(action.ScopeId, instance => instance with
        {
            {PluralName} = instance.{PluralName} with
            {
                Loading = true
            }
        });

    [ReducerMethod]
    public static {FeatureName}FeatureState ReduceResult(
        {FeatureName}FeatureState state,
        Load{PluralName}ResultAction action)
        => state.UpdateInstance(action.ScopeId, instance => instance with
        {
            {PluralName} = instance.{PluralName} with
            {
                Loading = false,
                Items = action.Items,
                TotalCount = action.TotalCount
            }
        });

    // ... other reducers follow the same pattern
}
```

**Rules:**
- Reducers are **pure functions** — no side effects, no async, no service calls.
- Always use `state.UpdateInstance(action.ScopeId, ...)` to route updates to the correct scope.
- Always use `with` expressions to produce new immutable state.
- Method name convention: `Reduce{ActionPurpose}` (e.g. `ReduceLoad`, `ReduceResult`, `ReduceEditProvided`).

### 5. Effects (`Store/{SubArea}/{SubArea}Effects.cs`)

Non-static class injected with WolverineFx `IMessageBus`. Handles async side effects:

```csharp
using Fluxor;
using Wolverine;

namespace GestioneCondominio.Web.Features.{FeatureName}.Store.{SubArea};

public class {SubArea}Effects(IMessageBus bus)
{
    [EffectMethod]
    public async Task HandleLoad(
        Load{PluralName}Action action,
        IDispatcher dispatcher)
    {
        var result = await bus.InvokeAsync<Get{PluralName}Response>(
            new Get{PluralName}Query(action.Filter));

        dispatcher.Dispatch(
            new Load{PluralName}ResultAction(action.ScopeId, result.Items, result.TotalCount));
    }

    [EffectMethod]
    public async Task HandleSave(
        Save{Name}RequestedAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var result = await bus.InvokeAsync<{Name}ResponseDto>(
                new Create{Name}Command(action.Item));

            dispatcher.Dispatch(new Save{Name}SuccessAction(action.ScopeId, result));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new Save{Name}FailedAction(action.ScopeId, ex.Message));
        }
    }
}
```

**Rules:**
- Effects are the **only** place where async operations and service calls happen.
- Always propagate `action.ScopeId` to dispatched result actions.
- Use WolverineFx `IMessageBus` to send commands/queries to the Application layer.
- Wrap save/mutation operations in try/catch, dispatching both success and failure actions.

### 6. Container Page (`Pages/{FeatureName}Page.razor` + `.razor.cs`)

**Razor markup:**
```razor
@page "/{FeatureName}"

@using GestioneCondominio.Web.Features.{FeatureName}.Store
@using Iqera.Net.Blazor.Fluxor.Components
@using Microsoft.AspNetCore.Authorization

@inherits ScopedFluxorContainerBase<{FeatureName}FeatureState, {FeatureName}FeatureInstanceState>

@attribute [Authorize]

@if (CurrentInstance is not null)
{
    <{FeatureName}List ScopeId="@this.ScopeId"></{FeatureName}List>

    @if (CurrentInstance.{PluralName}.Loading)
    {
        <MudProgressLinear Indeterminate="true" />
    }

    <MudDialog @bind-Visible="@CurrentInstance.{PluralName}.EditDialogOpen">
        <DialogContent>
            <{FeatureName}Edit ScopeId="@this.ScopeId"></{FeatureName}Edit>
        </DialogContent>
    </MudDialog>
}
```

**Code-behind:**
```csharp
using GestioneCondominio.Web.Features.{FeatureName}.Store;
using GestioneCondominio.Web.Features.{FeatureName}.Store.{SubArea};
using Iqera.Net.Blazor.Fluxor.Components;

namespace GestioneCondominio.Web.Features.{FeatureName}.Pages;

public partial class {FeatureName}Page
    : ScopedFluxorContainerBase<{FeatureName}FeatureState, {FeatureName}FeatureInstanceState>
{
    private void OnCloseDialog()
    {
        Dispatcher.Dispatch(new Close{Name}EditDialogAction(this.ScopeId));
    }
}
```

**Rules:**
- Inherit from `ScopedFluxorContainerBase<TState, TInstance>` (2 type params only).
- **No override methods needed** — init/dispose are handled automatically.
- Always guard rendering with `@if (CurrentInstance is not null)`.
- Pass `ScopeId` to all child components.

### 7. Child Components (`Components/{FeatureName}List.razor.cs`, etc.)

```csharp
using GestioneCondominio.Web.Features.{FeatureName}.Store;
using GestioneCondominio.Web.Features.{FeatureName}.Store.{SubArea};
using Iqera.Net.Blazor.Fluxor.Components;

namespace GestioneCondominio.Web.Features.{FeatureName}.Components;

public partial class {FeatureName}List
    : ScopedComponentBase<{FeatureName}FeatureState, {FeatureName}FeatureInstanceState>
{
    protected override async Task OnInitializedAsync()
    {
        // Subscribe to actions filtered by ScopeId
        SubscribeToScopedAction<Load{PluralName}ResultAction>(_ =>
        {
            StateHasChanged();
        });

        SubscribeToScopedAction<Save{Name}SuccessAction>(_ =>
        {
            StateHasChanged();
        });

        await base.OnInitializedAsync();
    }

    // Access state via Instance property
    // Instance.{PluralName}.Items, Instance.{PluralName}.Loading, etc.

    public void OnEditButton({Dto} item)
    {
        // Always include this.ScopeId in dispatched actions
        Dispatcher.Dispatch(new Edit{Name}RequestedAction(this.ScopeId, item));
    }
}
```

**Rules:**
- Inherit from `ScopedComponentBase<TState, TInstance>`.
- `ScopeId` is received as `[Parameter, EditorRequired]` from the container — never create it here.
- Access scoped state via the `Instance` property (auto-updated on state changes).
- Use `SubscribeToScopedAction<T>()` to react to specific actions — it automatically filters by `ScopeId`.
- Always include `this.ScopeId` when dispatching actions.
- **Do not** call `ActionSubscriber` directly — use the `SubscribeToScopedAction` helper.
- Unsubscription is handled automatically in `Dispose` via `ActionSubscriber.UnsubscribeFromAllActions(this)`.

---

## Embedding a Feature Inside Another Feature

A child component from Feature A can initialize and manage a scoped instance of Feature B (e.g. PropertyUnits inside CondominiumDetail):

```csharp
using Iqera.Net.Blazor.Fluxor.Abstractions;
using GestioneCondominio.Web.Features.PropertyUnits.Store;

// In OnInitializedAsync — initialize the nested feature scope
Dispatcher.Dispatch(new InitializeScopedAction<PropertyUnitsFeatureState>(this.ScopeId));

// In Dispose — clean up the nested feature scope
Dispatcher.Dispatch(new DisposeScopedAction<PropertyUnitsFeatureState>(this.ScopeId));
```

Use the generic `InitializeScopedAction<TState>` / `DisposeScopedAction<TState>` — never create custom init/dispose actions.

---

## Common Mistakes to Avoid

| Mistake | Correct approach |
|---------|-----------------|
| Creating `Initialize{Feature}ScopeAction` / `Dispose{Feature}ScopeAction` records | Use generic `InitializeScopedAction<TState>` / `DisposeScopedAction<TState>` |
| Creating a `Base{Feature}Reducer` with init/dispose methods | Use `{Feature}LifecycleReducer : ScopedLifecycleReducer<...>` marker |
| Inheriting `ScopedFluxorContainerBase` with 4 type params | Use the 2-param version `<TState, TInstance>` |
| Overriding `CreateInitializeAction` / `CreateDisposeAction` | Not needed with the 2-param container |
| Dispatching actions without `ScopeId` | Every action must include `ScopeId` |
| Calling `ActionSubscriber.SubscribeToAction` directly | Use `SubscribeToScopedAction<T>()` which auto-filters by scope |
| Calling services inside reducers | Reducers are pure — use effects for async/service calls |
| Using mutable properties in state records | Always use `{ get; init; }` and `with` expressions |
| Forgetting `@if (CurrentInstance is not null)` in container razor | State is null before init action is processed |