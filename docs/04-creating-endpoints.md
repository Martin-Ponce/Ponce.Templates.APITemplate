# Creating Endpoints

This covers the **Application** layer (DTOs + the service that holds your business logic) and the **API**
layer (the actual minimal-API HTTP endpoints), and how to wire a new feature into the app.

> Prerequisite: the entity/validator/repository contract exist (see
> [Creating Entities](02-creating-entities.md)) and the entity is mapped, has a `DbSet`, and its
> repository is implemented (see [Mapping Entities to the Database](03-mapping-entities-to-providers.md)).

There are no domain services in this template — **all business logic lives in the application service**,
which talks directly to the repository. See `Application/Services/ExampleItemService.cs` for the pattern
used throughout.

## 1. Request/response DTOs

Under `Application/Dtos/Request/<Feature>s/` and `Application/Dtos/Response/<Feature>s/`. These are the
transport shapes — plain classes, no behavior, no validation (validation lives on the entity, see
[Creating Entities](02-creating-entities.md#3-write-the-validator)).

```csharp
// Application/Dtos/Request/ExampleItems/SaveExampleItemRequest.cs
public class SaveExampleItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
```

Add one response class per shape you return. `ExampleItem` has three: `GetExampleItemResponse` (single),
`GetExampleItemsResponse` (list item — here identical to the single shape, but kept as its own class so
the list projection can diverge later, e.g. to omit a heavy field), and `SaveExampleItemResponse` (what
`POST` returns). All three are simple `Id`/`Name`/`Description` classes with matching property names to
the entity, which is what lets Mapster (`.Adapt<T>()` / `.ProjectToType<T>()`) map them automatically with
no configuration.

## 2. The application service

One class per feature under `Application/Services/`, taking the feature's `I<Entity>Repository` (and
`ILogger<T>`, if you want structured logging — the convention here is `LogDebug` before an operation,
`LogInformation` after it succeeds) as constructor parameters:

```csharp
// Application/Services/ExampleItemService.cs
using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Application.Dtos.Request.ExampleItems;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Application.Services;

public class ExampleItemService(IExampleItemRepository exampleItemRepository, ILogger<ExampleItemService> logger)
{
    private readonly IExampleItemRepository _exampleItemRepository = exampleItemRepository;
    private readonly ILogger<ExampleItemService> _logger = logger;

    public IQueryable<GetExampleItemsResponse> GetExampleItems() =>
        _exampleItemRepository.QueryExampleItems().ProjectToType<GetExampleItemsResponse>();

    public async Task<GetExampleItemResponse> GetExampleItem(Guid exampleItemId, CancellationToken ct = default)
    {
        var exampleItem = await GetExampleItemOrThrow(exampleItemId, ct);
        return exampleItem.Adapt<GetExampleItemResponse>();
    }

    public async Task<SaveExampleItemResponse> SaveExampleItem(SaveExampleItemRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Saving example item {ExampleItemName}", request.Name);
        var exampleItem = new ExampleItem(request.Name, request.Description);
        await _exampleItemRepository.SaveExampleItem(exampleItem, ct);
        await _exampleItemRepository.SaveChanges(ct);
        _logger.LogInformation("Example item {ExampleItemId} saved successfully", exampleItem.Id);
        return exampleItem.Adapt<SaveExampleItemResponse>();
    }

    private async Task<ExampleItem> GetExampleItemOrThrow(Guid exampleItemId, CancellationToken ct) =>
        await _exampleItemRepository.GetExampleItemById(exampleItemId, ct) ??
            throw new NotFoundException(
                CustomExceptionCodes.EXAMPLEITEM_NOT_FOUND,
                $"ExampleItem with id {exampleItemId} doesn't exist",
                new Dictionary<string, object> { { "ExampleItemId", exampleItemId } });
}
```

Points to note:

- `GetExampleItems()` returns `IQueryable<T>`, not a materialized list — the endpoint decides paging,
  filtering, and sorting (step 3), and only then does the query actually execute.
- The constructor entity (`new ExampleItem(...)`) is where domain validation runs — if `request.Name` is
  invalid, the `ValidationException` from the entity's validator propagates up untouched; there's no
  try/catch here (see `Middlewares/ErrorHandlerMiddleware.cs` for how exceptions become HTTP responses).
- `SaveChanges` is called explicitly once, after staging the change — this is the one place a
  create/update/delete actually commits. A method that needs multiple repository calls to succeed or fail
  together would call `SaveChanges` once at the end, after all of them.
- `GetExampleItemOrThrow` centralizes the not-found check (using the `EXAMPLEITEM_NOT_FOUND` code from
  [Creating Entities](02-creating-entities.md#2-add-length-limits-and-if-needed-an-error-code)) so every
  method that needs to load-then-act (update/toggle/delete) reuses it instead of repeating the null check.

## 3. The endpoints

Under `API/Endpoints/<Feature>s/`, **one static class per endpoint** (not per feature) — this keeps each
route's OpenAPI metadata, request binding, and status codes next to the one thing it does.

### List (with paging/sorting/filtering)

```csharp
// API/Endpoints/ExampleItems/GetExampleItems.cs
using Ponce.Templates.APITemplate.API.Dtos.Pagination;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Application.Services;
using Ponce.Templates.APITemplate.Domain.Exceptions;

namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class GetExampleItems
{
    public const string ENDPOINT_NAME = "Get example items";

    public static RouteGroupBuilder MapGetExampleItems(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapGet("",
            async (HttpContext context, ExampleItemService exampleItemService, [AsParameters] PaginationRequest request) =>
            {
                var exampleItems = exampleItemService.GetExampleItems();
                var response = await Operation(exampleItems, request);
                context.AddPaginationHeader(response);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<IEnumerable<GetExampleItemsResponse>>(StatusCodes.Status200OK)
        .WithDescription("Gets the list of example items in the system.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }

    private static IReadOnlyDictionary<string, IOrderedQueryable<GetExampleItemsResponse>> GetExampleItemsSorts(IQueryable<GetExampleItemsResponse> query) => new Dictionary<string, IOrderedQueryable<GetExampleItemsResponse>>()
    {
        { "name", query.OrderBy(a => a.Name) },
        { "name_desc", query.OrderByDescending(a => a.Name) },
    };

    private static async Task<PagedList<GetExampleItemsResponse>> Operation(IQueryable<GetExampleItemsResponse> query, PaginationRequest paginationRequest)
    {
        if (!string.IsNullOrEmpty(paginationRequest.Filter))
        {
            var filter = paginationRequest.Filter.ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(filter));
        }
        if (!string.IsNullOrEmpty(paginationRequest.Sort))
        {
            if (!GetExampleItemsSorts(query).TryGetValue(paginationRequest.Sort, out var sortedQuery))
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            query = sortedQuery;
        }
        return await PagedList<GetExampleItemsResponse>.ToPagedList(query, paginationRequest);
    }
}
```

`PaginationRequest`/`PagedList<T>`/`AddPaginationHeader` (all under `API/Dtos/Pagination/` and
`API/Helpers/PaginationHeaderUtil.cs`) are shared, reusable infrastructure — every list endpoint follows
this exact shape: bind `[AsParameters] PaginationRequest`, apply filter/sort via a small private
`Sorts`+`Operation` pair, call `PagedList<T>.ToPagedList`, add the `X-Pagination` response header. Add a
`{ "field", query.OrderBy(...) }` / `{ "field_desc", ... }` pair to the sorts dictionary for every sortable
property on your response DTO.

### Get by id (with the 404 path)

```csharp
// API/Endpoints/ExampleItems/GetExampleItemById.cs
using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class GetExampleItemById
{
    public const string ENDPOINT_NAME = "Get example item by id";

    public static RouteGroupBuilder MapGetExampleItemById(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapGet("{exampleItemId:guid}",
            async ([FromRoute] Guid exampleItemId, ExampleItemService exampleItemService, CancellationToken ct) =>
            {
                var response = await exampleItemService.GetExampleItem(exampleItemId, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<GetExampleItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Gets a specific example item by id.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
```

Note there's no try/catch for the 404 here either — `GetExampleItemOrThrow` (step 2) throws
`NotFoundException`, and `Middlewares/ErrorHandlerMiddleware.cs` maps that exception type to HTTP 404
globally. The endpoint just declares `.Produces(StatusCodes.Status404NotFound)` so Swagger documents it.

### Create

```csharp
// API/Endpoints/ExampleItems/SaveExampleItem.cs
using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Request.ExampleItems;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class SaveExampleItem
{
    public const string ENDPOINT_NAME = "Create example item";

    public static RouteGroupBuilder MapSaveExampleItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapPost("",
            async ([FromBody] SaveExampleItemRequest request, ExampleItemService exampleItemService, CancellationToken ct) =>
            {
                var response = await exampleItemService.SaveExampleItem(request, ct);
                return Results.Created($"exampleitems/{response.Id}", response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<SaveExampleItemResponse>(StatusCodes.Status201Created)
        .WithDescription("Creates a new example item.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
```

Every endpoint class follows the same skeleton: a `public const string ENDPOINT_NAME`, a
`Map<Verb><Feature>` extension method on `RouteGroupBuilder` that maps the route and chains
`.HasApiVersion(...)`, `.Produces<T>(...)` per possible status code, `.WithDescription/.WithSummary/.WithName`.
Reuse `[FromRoute]`/`[FromBody]`/`[AsParameters]` bindings as appropriate, inject the feature's application
service directly as a handler parameter (constructor-injected services aren't needed for minimal APIs —
the DI container resolves handler parameters per-request), and always accept a trailing
`CancellationToken ct` for any endpoint that awaits I/O.

## 4. The endpoint group

One per feature, under `API/Endpoints/<Feature>s/<Feature>EndpointGroup.cs`, mapping a route group and
chaining every endpoint's `Map...` method onto it:

```csharp
// API/Endpoints/ExampleItems/ExampleItemEndpointGroup.cs
namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class ExampleItemEndpointGroup
{
    public static RouteGroupBuilder MapExampleItemEndpoints(this RouteGroupBuilder appEndpoints)
    {
        var exampleItemEndpointGroup = appEndpoints.MapGroup("exampleitems").WithTags("ExampleItems");
        exampleItemEndpointGroup
            .MapGetExampleItems()
            .MapGetExampleItemById()
            .MapSaveExampleItem();
        return appEndpoints;
    }
}
```

`MapGroup("exampleitems")` becomes the route prefix under `api/` (see step 6), and `.WithTags(...)` is the
Swagger UI grouping label.

## 5. Register the repository and service in DI

In [`API/Extensions/ServiceExtensions.cs`](../Ponce.Templates.APITemplate.API/Extensions/ServiceExtensions.cs),
inside `ConfigureServices`, add one `AddScoped` line for the repository and one for the service:

```csharp
public static IServiceCollection ConfigureServices(this IServiceCollection services, GeneralConfiguration configuration)
{
    services.AddScoped<IRepository, Repository<AppDbContext>>();

    // Repositories
    services.AddScoped<IExampleItemRepository, ExampleItemRepository>();
    // Add one services.AddScoped<I<Feature>Repository, <Feature>Repository>() call per feature.

    // Application services
    services.AddScoped<ExampleItemService>();
    // Add one services.AddScoped<<Feature>Service>() call per feature.
    return services;
}
```

Both are registered `Scoped` — one instance per HTTP request, matching `AppDbContext`'s own scoped
lifetime (a repository and the context it wraps must share the same scope, or you'll get a different
`DbContext` instance than the one your changes were staged on).

## 6. Wire the endpoint group into the route table

In [`API/Extensions/WebApplicationExtensions.cs`](../Ponce.Templates.APITemplate.API/Extensions/WebApplicationExtensions.cs),
inside `MapEndpoints`, add the `using` for your endpoint group's namespace and chain its `Map<Feature>Endpoints()`
call:

```csharp
using Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;
// ...

public static WebApplication MapEndpoints(this WebApplication app, string rateLimitingPolicy)
{
    var versionSet = app.GetVersionSet();
    var apiEndpoints = app.MapGroup("api")
                          .WithApiVersionSet(versionSet)
                          .RequireRateLimiting(rateLimitingPolicy)
                          .AddOpenApi();
    // Once you add authentication back, chain `.RequireAuthorization()` here too.
    apiEndpoints
        .MapExampleItemEndpoints();
    // Add every new feature's `.Map<Feature>Endpoints()` call here.
    return app;
}
```

This is what puts your feature's routes under `api/exampleitems` (the `"api"` prefix comes from
`MapGroup("api")` here, the `"exampleitems"` segment from `MapGroup("exampleitems")` in the endpoint
group, step 4).

## 7. Try it

```powershell
cd Ponce.Templates.APITemplate.API
dotnet run
```

Then (with `X-Version: 1.0` on every call, per [Getting Started](01-getting-started.md#running-the-api)):

```bash
curl -X POST http://localhost:5008/api/exampleitems \
  -H "Content-Type: application/json" -H "X-Version: 1.0" \
  -d '{"name":"Test Item","description":"A description"}'

curl http://localhost:5008/api/exampleitems -H "X-Version: 1.0"

curl http://localhost:5008/api/exampleitems/<id-from-the-post-response> -H "X-Version: 1.0"
```

Or just open `http://localhost:5008/swagger` and try it from there — your new feature shows up as its own
tag, grouped by the `.WithTags(...)` call from step 4.

## Recap: adding a whole new feature end-to-end

1. [Creating Entities](02-creating-entities.md): entity + validator + `<Entity>Limits`/`<ENTITY>_NOT_FOUND` + repository contract.
2. [Mapping Entities to the Database](03-mapping-entities-to-providers.md): one mapping class per provider, registered in each provider strategy, `DbSet` added to `AppDbContext`, repository implemented.
3. This guide: request/response DTOs, application service, endpoint classes, endpoint group, DI registration, route wiring.
