using Mapster;
using Ponce.Templates.APITemplate.Application.Dtos.Request.Items;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Domain.Services;

namespace Ponce.Templates.APITemplate.Application.Services;

/// <summary>
/// Sample application service: translates between transport DTOs and the domain model, and is
/// what the API layer's endpoints call into. Copy this shape for every real feature you add.
/// </summary>
public class ItemService(ItemDomainService domainService)
{
    private readonly ItemDomainService _domainService = domainService;

    public IQueryable<GetItemsResponse> GetItems() =>
        _domainService.QueryItems().ProjectToType<GetItemsResponse>();

    public async Task<GetItemResponse> GetItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await _domainService.GetItem(itemId, ct);
        return item.Adapt<GetItemResponse>();
    }

    public async Task<SaveItemResponse> SaveItem(SaveItemRequest request, CancellationToken ct = default)
    {
        var item = await _domainService.SaveItem(request.Name, request.Description, ct);
        return item.Adapt<SaveItemResponse>();
    }

    public async Task<UpdateItemResponse> UpdateItem(Guid itemId, UpdateItemRequest request, CancellationToken ct = default)
    {
        var item = await _domainService.UpdateItem(itemId, request.Name, request.Description, ct);
        return item.Adapt<UpdateItemResponse>();
    }

    public async Task<ToggleItemResponse> ToggleItem(Guid itemId, CancellationToken ct = default)
    {
        var item = await _domainService.ToggleItem(itemId, ct);
        return item.Adapt<ToggleItemResponse>();
    }

    public async Task<DeleteItemResponse> DeleteItem(Guid itemId, CancellationToken ct = default)
    {
        await _domainService.DeleteItem(itemId, ct);
        return new DeleteItemResponse { Id = itemId };
    }
}
