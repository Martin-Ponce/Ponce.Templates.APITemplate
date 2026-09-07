using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Request.Items;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class UpdateItem
{
    public const string ENDPOINT_NAME = "Update item";

    public static RouteGroupBuilder MapUpdateItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapPut("{itemId:guid}",
            async ([FromRoute] Guid itemId, [FromBody] UpdateItemRequest request, ItemService itemService, CancellationToken ct) =>
            {
                var response = await itemService.UpdateItem(itemId, request, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<UpdateItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Updates an existing item's parameters.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
