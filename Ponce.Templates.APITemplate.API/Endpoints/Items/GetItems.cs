using Ponce.Templates.APITemplate.API.Dtos.Pagination;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;
using Ponce.Templates.APITemplate.Domain.Exceptions;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class GetItems
{
    public const string ENDPOINT_NAME = "Get items";

    public static RouteGroupBuilder MapGetItems(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapGet("",
            async (HttpContext context, ItemService itemService, [AsParameters] PaginationRequest request) =>
            {
                var items = itemService.GetItems();
                var response = await Operation(items, request);
                context.AddPaginationHeader(response);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<IEnumerable<GetItemsResponse>>(StatusCodes.Status200OK)
        .WithDescription("Gets the list of items in the system.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }

    private static IReadOnlyDictionary<string, IOrderedQueryable<GetItemsResponse>> GetItemsSorts(IQueryable<GetItemsResponse> query) => new Dictionary<string, IOrderedQueryable<GetItemsResponse>>()
    {
        { "name", query.OrderBy(a => a.Name) },
        { "name_desc", query.OrderByDescending(a => a.Name) },
        { "enabled", query.OrderBy(a => a.Enabled) },
        { "enabled_desc", query.OrderByDescending(a => a.Enabled) },
    };

    private static async Task<PagedList<GetItemsResponse>> Operation(IQueryable<GetItemsResponse> query, PaginationRequest paginationRequest)
    {
        if (!string.IsNullOrEmpty(paginationRequest.Filter))
        {
            var filter = paginationRequest.Filter.ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(filter));
        }
        if (!string.IsNullOrEmpty(paginationRequest.Sort))
        {
            if (!GetItemsSorts(query).TryGetValue(paginationRequest.Sort, out var sortedQuery))
            {
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            }
            query = sortedQuery;
        }
        return await PagedList<GetItemsResponse>.ToPagedList(query, paginationRequest);
    }
}
