using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;

namespace Ponce.Templates.APITemplate.Infrastructure.Repositories;

public class ItemRepository(AppDbContext context, ILogger<ItemRepository> logger)
    : Repository<AppDbContext>(context), IItemRepository
{
    private readonly ILogger<ItemRepository> _logger = logger;

    public IQueryable<Item> QueryItems()
    {
        _logger.LogDebug("Querying items");
        return _context.Items;
    }

    public async Task<Item?> GetItemById(Guid itemId, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching item with id {ItemId}", itemId);
        return await _context.Items.FirstOrDefaultAsync(x => x.Id == itemId, ct);
    }

    public async Task SaveItem(Item item, CancellationToken ct = default)
    {
        _logger.LogDebug("Saving item with id {ItemId}", item.Id);
        await _context.Items.AddAsync(item, ct);
    }

    public void DeleteItem(Item item)
    {
        _logger.LogDebug("Deleting item with id {ItemId}", item.Id);
        _context.Items.Remove(item);
    }
}
