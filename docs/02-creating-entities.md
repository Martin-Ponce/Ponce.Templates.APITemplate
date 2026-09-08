# Creating Entities

This covers the **Domain** layer: the entity itself, its validation, its length/error-code constants,
and the repository contract the rest of the app will use to persist it. Everything here lives in
`Ponce.Templates.APITemplate.Domain` and has zero dependencies on EF Core, ASP.NET Core, or any other
project in the solution.

We'll use the real `ExampleItem` feature already in the codebase as the walkthrough example.

## 1. Pick a base class

Every entity derives from one of two base classes in `Domain/Entities/Base/`:

- **`AggregateRoot`** — for the "root" of a feature, the thing a repository loads/saves directly. Its
  `Id` has a `protected set`, so a derived class can assign it explicitly (a generated `Guid`, or a
  natural/business key like a code string).
- **`Entity`** — for a child/owned object that only ever exists attached to an aggregate root. Its `Id`
  is a `Guid` generated automatically in the constructor and can't be reassigned.

Almost everything you add will be an `AggregateRoot`. `ExampleItem` is one:

```csharp
// Domain/Entities/Base/AggregateRoot.cs
public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }

    protected AggregateRoot()
    {
        Id = Guid.Empty;
    }
}
```

## 2. Add length limits and (if needed) an error code

Open [`Domain/DomainConstants.cs`](../Ponce.Templates.APITemplate.Domain/DomainConstants.cs). Two things
usually need an entry per entity:

**A nested `<Entity>Limits` class** under `DomainConstants.Values`, holding every length-bound property's
min/max — these get reused both by the entity's validator (below) and by every provider's EF Core mapping
(see [Mapping Entities to the Database](03-mapping-entities-to-providers.md)), so there's exactly one
place to change a limit:

```csharp
public static class Values
{
    public const int MIN_LENGTH_VALUE = 3;

    public static class ExampleItemLimits
    {
        public const int MIN_LENGTH_NAME = MIN_LENGTH_VALUE;
        public const int MAX_LENGTH_NAME = 128;
        public const int MAX_LENGTH_DESCRIPTION = 512;
    }
    // Add one nested <Entity>Limits class per entity with length-bound properties.
}
```

**A `<ENTITY>_NOT_FOUND` constant** under `DomainConstants.CustomExceptionCodes`, if your feature will ever
need to look an instance up by id (almost always true) — this is what a "get by id" endpoint's 404 uses:

```csharp
public static class CustomExceptionCodes
{
    #region Not Found
    public const int EXAMPLEITEM_NOT_FOUND = 1;
    // Add one <ENTITY>_NOT_FOUND = <n> constant per aggregate root that can be missing.
    #endregion

    #region Invalid Operation
    // Add one <ENTITY>_ALREADY_EXISTS = <n> constant per aggregate root with a uniqueness invariant.
    #endregion
}
```

Keep the numeric codes unique **within their own region** (they don't need to be globally unique — they're
scoped by exception type, since `NotFoundException`/`InvalidOperationAppException`/etc. are distinguished
by their .NET type as well as this code).

## 3. Write the validator

One static class per entity under `Domain/Validators/Entities/`, with one extension method per validated
property. These call into the generic helpers in
[`Domain/Validators/StringValidator.cs`](../Ponce.Templates.APITemplate.Domain/Validators/StringValidator.cs)
(`ValidateLength`, `ValidateNull`, `ValidateRegex`), which throw a `ValidationException` (HTTP 400) when a
value is out of range:

```csharp
// Domain/Validators/Entities/ExampleItemValidator.cs
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;

namespace Ponce.Templates.APITemplate.Domain.Validators.Entities;

public static class ExampleItemValidator
{
    public static string ValidateName(this string? name) =>
        name.ValidateLength(
            ExampleItemLimits.MAX_LENGTH_NAME,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(name), ExampleItemLimits.MAX_LENGTH_NAME),
            ExampleItemLimits.MIN_LENGTH_NAME);

    public static string? ValidateDescription(this string? description)
    {
        if (string.IsNullOrEmpty(description))
            return description;
        return description.ValidateLength(
            ExampleItemLimits.MAX_LENGTH_DESCRIPTION,
            StringValidator.ExceptionWhenOutOfRangeArguments(nameof(description), ExampleItemLimits.MAX_LENGTH_DESCRIPTION));
    }
}
```

A required field (like `Name`) validates unconditionally. An optional field (like `Description`) first
checks `IsNullOrEmpty` and returns as-is if so — only a *non-empty* value gets length-checked.

## 4. Write the entity

Under `Domain/Entities/<Category>/`, one class per entity:

- Every settable property uses `private set` — the only way to change state is through the constructor or
  an explicit method, so invariants can never be bypassed.
- The constructor calls the validator extension methods from step 3.
- Add a parameterless `private` constructor with a comment `// Required by EF Core` — EF Core needs it to
  materialize instances from query results, but nothing else should ever call it.
- Add one public method per state transition your feature needs (e.g. `Update(...)`). `ExampleItem` only
  needs creation, so it has none.

```csharp
// Domain/Entities/Example/ExampleItem.cs
using Ponce.Templates.APITemplate.Domain.Entities.Base;
using Ponce.Templates.APITemplate.Domain.Validators.Entities;

namespace Ponce.Templates.APITemplate.Domain.Entities.Example;

public class ExampleItem : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private ExampleItem() { } // Required by EF Core

    public ExampleItem(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name.ValidateName();
        Description = description.ValidateDescription();
    }
}
```

If your entity needs an `Update`-style method later, follow the same shape as it existed for the old
`Item` sample entity:

```csharp
public void Update(string name, string? description)
{
    Name = name.ValidateName();
    Description = description.ValidateDescription();
}
```

## 5. Write the repository contract

One interface per aggregate root under `Domain/Contracts/`, extending the generic
[`IRepository`](../Ponce.Templates.APITemplate.Domain/Contracts/IRepository.cs) (which only provides
`SaveChanges`/`BeginTransaction`/`CloseTransaction` — the plumbing every repository needs). Declare only
the query/command methods your feature actually needs:

```csharp
// Domain/Contracts/IExampleItemRepository.cs
using Ponce.Templates.APITemplate.Domain.Entities.Example;

namespace Ponce.Templates.APITemplate.Domain.Contracts;

public interface IExampleItemRepository : IRepository
{
    IQueryable<ExampleItem> QueryExampleItems();
    Task<ExampleItem?> GetExampleItemById(Guid exampleItemId, CancellationToken ct = default);
    Task SaveExampleItem(ExampleItem exampleItem, CancellationToken ct = default);
}
```

A few conventions worth keeping:

- `Query<Entities>()` returns `IQueryable<T>` — filtering/paging/projection happens later, in the
  application service or endpoint, not here.
- `Get<Entity>ById(...)` returns a nullable `Task<T?>` — the caller decides what "not found" means (see
  [Creating Endpoints](04-creating-endpoints.md), which throws `NotFoundException` using the
  `<ENTITY>_NOT_FOUND` code from step 2).
- Mutating methods (`Save...`, `Delete...`) only stage the change on the `DbContext` — they never call
  `SaveChanges` themselves. The caller decides when to commit, which lets a service batch multiple
  repository calls into one transaction.

## Next steps

The interface from step 5 has no implementation yet, and the entity from step 4 has nowhere to be stored
yet. Continue to [Mapping Entities to the Database](03-mapping-entities-to-providers.md) to map the entity
to SqlServer/Postgres/Sqlite, implement the repository, and register a `DbSet` for it.

Once that's done, [Creating Endpoints](04-creating-endpoints.md) covers turning the repository into an
application service and HTTP endpoints.
