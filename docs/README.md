# Ponce.Templates.APITemplate

[Leer en español](#español)

## English
A template that follows my own architecture to use as a base in any new project, special thanks to my good friend Kiyoshi Shimizu, i owe him for many of the features i use in this template.

## Docs

These guides cover how the template is put together and how to extend it. Read them in order the
first time, after that just jump to whichever one you need.

| # | Guide | What it covers |
|---|-------|-----------------|
| 1 | [Getting Started](01-getting-started.md) | Project layout, configuration (appsettings/user-secrets/env vars), picking a database provider, running the API. |
| 2 | [Creating Entities](02-creating-entities.md) | The Domain layer: aggregate roots, validators, error codes/length limits, repository contracts. |
| 3 | [Mapping Entities to the Database](03-mapping-entities-to-providers.md) | EF Core config per provider (SqlServer/Postgres/Sqlite), the strategy pattern, hooking a `DbSet` into `AppDbContext`. |
| 4 | [Creating Endpoints](04-creating-endpoints.md) | The Application layer (DTOs + service), the API layer (minimal-API endpoints) and wiring a feature into the app. |
| 5 | [Running in a Container](05-docker-container.md) | Building/running the Docker image and passing connection strings and other settings through environment variables. |

## Architecture at a glance

It's four projects, one per layer, in a simple Clean/Onion style where each feature is a vertical slice:

```
Ponce.Templates.APITemplate.Domain          <- entities, validators, repository contracts, exceptions
Ponce.Templates.APITemplate.Application      <- DTOs, application services (all business logic lives here)
Ponce.Templates.APITemplate.Infrastructure   <- EF Core DbContext, entity mappings, repositories
Ponce.Templates.APITemplate.API              <- minimal-API endpoints, DI wiring, cross-cutting config
```

Dependencies only point inward: `API` → `Application`/`Infrastructure` → `Domain`. `Domain` doesn't depend
on anything else in the solution.

This is how a request travels through it:

```
HTTP request
  -> API/Endpoints/<Feature>/*.cs            (minimal-API route handler)
  -> Application/Services/<Feature>Service.cs (business logic, orchestrates the repository)
  -> Domain/Contracts/I<Feature>Repository.cs (the contract the service depends on)
  -> Infrastructure/Repositories/<Feature>Repository.cs (EF Core implementation)
  -> Infrastructure/Contexts/AppDbContext.cs  (the DbSet, mapped per-provider)
```

## The `ExampleItem` feature

All the guides use the same working feature that's already in the codebase: `ExampleItem` (`Id`, `Name`,
`Description`) with three endpoints (list, get-by-id, create). When a guide shows some code, it's pulled
straight from these files:

- Domain: [`Entities/Example/ExampleItem.cs`](../Ponce.Templates.APITemplate.Domain/Entities/Example/ExampleItem.cs), [`Validators/Entities/ExampleItemValidator.cs`](../Ponce.Templates.APITemplate.Domain/Validators/Entities/ExampleItemValidator.cs), [`Contracts/IExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Domain/Contracts/IExampleItemRepository.cs)
- Application: [`Services/ExampleItemService.cs`](../Ponce.Templates.APITemplate.Application/Services/ExampleItemService.cs), `Dtos/Request/ExampleItems/*`, `Dtos/Response/ExampleItems/*`
- Infrastructure: [`Repositories/ExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Infrastructure/Repositories/ExampleItemRepository.cs), `Mapping/SqlServer|Postgres|Sqlite/ExampleItem*Configuration.cs`
- API: `Endpoints/ExampleItems/*.cs`

## Adding a new feature, the short version

1. **[Create the entity](02-creating-entities.md)** in `Domain` (entity + validator + length limits + repository contract).
2. **[Map it to the database](03-mapping-entities-to-providers.md)** in `Infrastructure` (one `*Configuration` class per provider, registered in the provider strategies, plus the `DbSet` in `AppDbContext`).
3. **[Build the endpoints](04-creating-endpoints.md)** (`Application` DTOs + service, `API` endpoint classes + endpoint group), then register the repository/service in DI and the endpoint group in the route table.

Each guide walks through the steps with the exact code.

---

## Español

[English](#docs)

Una plantilla que sigue mi propia arquitectura para usarla como base en cualquier proyecto nuevo, mil gracias a mi buen amigo Kiyoshi Shimizu, le debo muchas de las características que uso aqui.

Las guías están en inglés, aquí va un resumen rápido para ubicarte.

### Guías

| # | Guía | De qué trata |
|---|------|--------------|
| 1 | [Getting Started](01-getting-started.md) | Estructura del proyecto, configuración (appsettings/user-secrets/variables de entorno), elegir el proveedor de base de datos y correr la API. |
| 2 | [Creating Entities](02-creating-entities.md) | La capa Domain: aggregate roots, validadores, códigos de error/límites de longitud y contratos de repositorio. |
| 3 | [Mapping Entities to the Database](03-mapping-entities-to-providers.md) | Configuración de EF Core por proveedor (SqlServer/Postgres/Sqlite), el patrón strategy y cómo meter un `DbSet` al `AppDbContext`. |
| 4 | [Creating Endpoints](04-creating-endpoints.md) | La capa Application (DTOs + servicio), la capa API (minimal APIs) y cómo conectar una funcionalidad a la app. |
| 5 | [Running in a Container](05-docker-container.md) | Construir y correr la imagen de Docker y pasar connection strings y demás ajustes con variables de entorno. |

### Arquitectura en corto

Son cuatro proyectos, uno por capa, en un estilo Clean/Onion sencillo donde cada funcionalidad es un vertical slice:

```
Ponce.Templates.APITemplate.Domain          <- entidades, validadores, contratos de repositorio, excepciones
Ponce.Templates.APITemplate.Application      <- DTOs, servicios de aplicación (aquí vive la lógica de negocio)
Ponce.Templates.APITemplate.Infrastructure   <- DbContext de EF Core, mapeos de entidades, repositorios
Ponce.Templates.APITemplate.API              <- endpoints minimal API, inyección de dependencias, configuración general
```

Las dependencias solo apuntan hacia adentro: `API` → `Application`/`Infrastructure` → `Domain`. `Domain` no depende de nada más de la solución.

Así viaja un request:

```
Request HTTP
  -> API/Endpoints/<Feature>/*.cs            (handler de la ruta)
  -> Application/Services/<Feature>Service.cs (lógica de negocio, orquesta el repositorio)
  -> Domain/Contracts/I<Feature>Repository.cs (el contrato del que depende el servicio)
  -> Infrastructure/Repositories/<Feature>Repository.cs (implementación con EF Core)
  -> Infrastructure/Contexts/AppDbContext.cs  (el DbSet, mapeado por proveedor)
```

### La funcionalidad `ExampleItem`

Todas las guías usan la misma funcionalidad que ya está funcionando en el código: `ExampleItem` (`Id`, `Name`, `Description`) con tres endpoints (listar, obtener por id y crear). Cuando una guía muestra código, sale directo de estos archivos:

- Domain: [`Entities/Example/ExampleItem.cs`](../Ponce.Templates.APITemplate.Domain/Entities/Example/ExampleItem.cs), [`Validators/Entities/ExampleItemValidator.cs`](../Ponce.Templates.APITemplate.Domain/Validators/Entities/ExampleItemValidator.cs), [`Contracts/IExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Domain/Contracts/IExampleItemRepository.cs)
- Application: [`Services/ExampleItemService.cs`](../Ponce.Templates.APITemplate.Application/Services/ExampleItemService.cs), `Dtos/Request/ExampleItems/*`, `Dtos/Response/ExampleItems/*`
- Infrastructure: [`Repositories/ExampleItemRepository.cs`](../Ponce.Templates.APITemplate.Infrastructure/Repositories/ExampleItemRepository.cs), `Mapping/SqlServer|Postgres|Sqlite/ExampleItem*Configuration.cs`
- API: `Endpoints/ExampleItems/*.cs`

### Agregar una funcionalidad nueva, la versión corta

1. **[Crea la entidad](02-creating-entities.md)** en `Domain` (entidad + validador + límites de longitud + contrato del repositorio).
2. **[Mapéala a la base de datos](03-mapping-entities-to-providers.md)** en `Infrastructure` (una clase `*Configuration` por proveedor, registrada en las strategies, más el `DbSet` en `AppDbContext`).
3. **[Arma los endpoints](04-creating-endpoints.md)** (DTOs y servicio en `Application`, clases de endpoint y grupo en `API`), y luego registra el repositorio/servicio en la inyección de dependencias y el grupo de endpoints en la tabla de rutas.

Cada guía te lleva paso a paso con el código exacto.
