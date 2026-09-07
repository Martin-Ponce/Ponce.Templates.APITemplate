using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class GetItem
{
    public const string ENDPOINT_NAME = "Get item";

    public static RouteGroupBuilder MapGetItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapGet("{itemId:guid}",
            async ([FromRoute] Guid itemId, ItemService itemService, CancellationToken ct) =>
            {
                var response = await itemService.GetItem(itemId, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<GetItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Gets a specific item by id.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
