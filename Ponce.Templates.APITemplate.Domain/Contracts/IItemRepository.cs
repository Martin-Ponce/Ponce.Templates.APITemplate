using Ponce.Templates.APITemplate.Domain.Entities.Sample;

namespace Ponce.Templates.APITemplate.Domain.Contracts;

/// <summary>
/// Sample repository contract for the "Item" vertical slice. Rename/copy this pattern
/// (one repository interface per aggregate root, implemented in the Infrastructure layer) for
/// every real feature you add.
/// </summary>
public interface IItemRepository : IRepository
{
    IQueryable<Item> QueryItems();
    Task<Item?> GetItemById(Guid itemId, CancellationToken ct = default);
    Task SaveItem(Item item, CancellationToken ct = default);
    void DeleteItem(Item item);
}
