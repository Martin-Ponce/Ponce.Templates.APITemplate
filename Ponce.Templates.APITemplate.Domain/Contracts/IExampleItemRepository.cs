using Ponce.Templates.APITemplate.Domain.Entities.Example;

namespace Ponce.Templates.APITemplate.Domain.Contracts;

/// <summary>Repository contract for the "ExampleItem" vertical slice, implemented in the Infrastructure layer.</summary>
public interface IExampleItemRepository : IRepository
{
    IQueryable<ExampleItem> QueryExampleItems();
    Task<ExampleItem?> GetExampleItemById(Guid exampleItemId, CancellationToken ct = default);
    Task SaveExampleItem(ExampleItem exampleItem, CancellationToken ct = default);
}
