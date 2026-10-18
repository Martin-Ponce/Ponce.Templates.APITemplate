using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Application.Dtos.Request.ExampleItems;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Application.Services;

/// <summary>Business logic for example items, maps between DTOs and the domain model.</summary>
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
