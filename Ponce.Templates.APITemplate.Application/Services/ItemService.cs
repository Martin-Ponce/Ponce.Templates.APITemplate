using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Application.Dtos.Request.Items;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Application.Services;

/// <summary>
/// Sample application service: owns all business logic for the "Item" feature (existence checks,
/// orchestration of repository calls + SaveChanges, logging) and translates between transport DTOs
/// and the domain model. It's what the API layer's endpoints call into. Copy this shape for every
/// real feature you add.
/// </summary>
public class ItemService(IItemRepository itemRepository, ILogger<ItemService> logger)
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly ILogger<ItemService> _logger = logger;

    public IQueryable<GetItemsResponse> GetItems() =>
        _itemRepository.QueryItems().ProjectToType<GetItemsResponse>();

    public async Task<GetItemResponse> GetItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await GetItemOrThrow(itemId, ct);
        return item.Adapt<GetItemResponse>();
    }

    public async Task<SaveItemResponse> SaveItem(SaveItemRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Saving item {ItemName}", request.Name);
        var item = new Item(request.Name, request.Description);
        await _itemRepository.SaveItem(item, ct);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} saved successfully", item.Id);
        return item.Adapt<SaveItemResponse>();
    }

    public async Task<UpdateItemResponse> UpdateItem(Guid itemId, UpdateItemRequest request, CancellationToken ct = default)
    {
        var item = await GetItemOrThrow(itemId, ct);
        item.Update(request.Name, request.Description);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} updated successfully", itemId);
        return item.Adapt<UpdateItemResponse>();
    }

    public async Task<ToggleItemResponse> ToggleItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await GetItemOrThrow(itemId, ct);
        item.Toggle();
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} toggled successfully", itemId);
        return item.Adapt<ToggleItemResponse>();
    }

    public async Task<DeleteItemResponse> DeleteItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await GetItemOrThrow(itemId, ct);
        _itemRepository.DeleteItem(item);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} deleted successfully", itemId);
        return new DeleteItemResponse { Id = itemId };
    }

    private async Task<Item> GetItemOrThrow(Guid itemId, CancellationToken ct) =>
        await _itemRepository.GetItemById(itemId, ct) ??
            throw new NotFoundException(
                CustomExceptionCodes.ITEM_NOT_FOUND,
                $"Item with id {itemId} doesn't exist",
                new Dictionary<string, object> { { "ItemId", itemId } });
}
