using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Domain.Services;

/// <summary>
/// Sample domain service: owns the business rules for the "Item" aggregate (existence checks,
/// invariants that span more than one entity, orchestration of repository calls + SaveChanges).
/// Copy this shape for every real aggregate you add, then delete this sample.
/// </summary>
public class ItemDomainService(IItemRepository itemRepository, ILogger<ItemDomainService> logger)
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly ILogger<ItemDomainService> _logger = logger;

    public IQueryable<Item> QueryItems() => _itemRepository.QueryItems();

    public async Task<Item> GetItem(Guid itemId, CancellationToken ct = default)
    {
        return await _itemRepository.GetItemById(itemId, ct) ??
            throw new NotFoundException(
                CustomExceptionCodes.ITEM_NOT_FOUND,
                $"Item with id {itemId} doesn't exist",
                new Dictionary<string, object> { { "ItemId", itemId } });
    }

    public async Task<Item> SaveItem(string name, string? description, CancellationToken ct = default)
    {
        _logger.LogDebug("Saving item {ItemName}", name);
        var item = new Item(name, description);
        await _itemRepository.SaveItem(item, ct);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} saved successfully", item.Id);
        return item;
    }

    public async Task<Item> UpdateItem(Guid itemId, string name, string? description, CancellationToken ct = default)
    {
        var item = await GetItem(itemId, ct);
        item.Update(name, description);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} updated successfully", itemId);
        return item;
    }

    public async Task<Item> ToggleItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await GetItem(itemId, ct);
        item.Toggle();
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} toggled successfully", itemId);
        return item;
    }

    public async Task DeleteItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await GetItem(itemId, ct);
        _itemRepository.DeleteItem(item);
        await _itemRepository.SaveChanges(ct);
        _logger.LogInformation("Item {ItemId} deleted successfully", itemId);
    }
}
