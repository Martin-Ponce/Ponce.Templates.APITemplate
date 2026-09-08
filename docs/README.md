# Ponce.Templates.APITemplate — Documentation

This folder documents how this template is put together and how to extend it. Read them in order the
first time; afterwards, jump straight to the one you need.

| # | Guide | What it covers |
|---|-------|-----------------|
| 1 | [Getting Started](01-getting-started.md) | Project layout, configuration (appsettings/user-secrets/env vars), choosing a database provider, running the API. |
| 2 | [Creating Entities](02-creating-entities.md) | Domain layer: aggregate roots, validators, error codes/length limits, repository contracts. |
| 3 | [Mapping Entities to the Database](03-mapping-entities-to-providers.md) | EF Core configuration per provider (SqlServer/Postgres/Sqlite), the configuration-strategy pattern, wiring a `DbSet` into `AppDbContext`. |
| 4 | [Creating Endpoints](04-creating-endpoints.md) | Application layer (DTOs + service) and API layer (minimal-API endpoints), and wiring a feature into the app. |
| 5 | [Running in a Container](05-docker-container.md) | Building/running the Docker image, and how connection strings and other settings are supplied via environment variables at run time. |

## Architecture at a glance

The solution is a single vertical-slice feature style spread across four projects, each one layer of a
simple Clean/Onion architecture:

```
Ponce.Templates.APITemplate.Domain          <- entities, validators, repository contracts, exceptions
Ponce.Templates.APITemplate.Application      <- DTOs, application services (all business logic lives here)
Ponce.Templates.APITemplate.Infrastructure   <- EF Core DbContext, entity mappings, repositories
Ponce.Templates.APITemplate.API              <- minimal-API endpoints, DI wiring, cross-cutting config
```

Dependencies only point inward: `API` → `Application`/`Infrastructure` → `Domain`. `Domain` depends on
nothing else in the solution.

A request flows like this:

```
HTTP request
  -> API/Endpoints/<Feature>/*.cs            (minimal-API route handler)
  -> Application/Services/<Feature>Service.cs (business logic, orchestrates the repository)
  -> Domain/Contracts/I<Feature>Repository.cs (the contract the service depends on)
  -> Infrastructure/Repositories/<Feature>Repository.cs (EF Core implementation)
  -> Infrastructure/Contexts/AppDbContext.cs  (the DbSet, mapped per-provider)
```

## Reference feature: `ExampleItem`

Every tutorial in this folder points at the same real, working feature already in the codebase:
`ExampleItem` (`Id`, `Name`, `Description`), with three endpoints — list, get-by-id, create. Whenever a
tutorial says "here's what this looks like", it's quoting the actual files below:

- Domain: [`Entities/Example/ExampleItem.cs`](../Ponce.Templates.APITemplate.Domain/Entities/Example/ExampleItem.cs), [`Validators/Entities/ExampleItemValidator.cs`](../Ponce.Templates.APITemplate.Domain/Validators/Entities/ExampleItemValidator.cs), [`Contracts/IExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Domain/Contracts/IExampleItemRepository.cs)
- Application: [`Services/ExampleItemService.cs`](../Ponce.Templates.APITemplate.Application/Services/ExampleItemService.cs), `Dtos/Request/ExampleItems/*`, `Dtos/Response/ExampleItems/*`
- Infrastructure: [`Repositories/ExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Infrastructure/Repositories/ExampleItemRepository.cs), `Mapping/SqlServer|Postgres|Sqlite/ExampleItem*Configuration.cs`
- API: `Endpoints/ExampleItems/*.cs`

## Adding a brand-new feature: the short version

1. **[Create the entity](02-creating-entities.md)** in `Domain` (entity + validator + length limits + repository contract).
2. **[Map it to the database](03-mapping-entities-to-providers.md)** in `Infrastructure` (one `*Configuration` class per provider, registered in the provider strategies, `DbSet` added to `AppDbContext`).
3. **[Build the endpoints](04-creating-endpoints.md)** (`Application` DTOs + service, `API` endpoint classes + endpoint group), then wire the repository/service into DI and the endpoint group into the route table.

Each linked guide walks through every step with the exact code.
