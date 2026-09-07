using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class DeleteItem
{
    public const string ENDPOINT_NAME = "Delete item";

    public static RouteGroupBuilder MapDeleteItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapDelete("{itemId:guid}",
            async ([FromRoute] Guid itemId, ItemService itemService, CancellationToken ct) =>
            {
                var response = await itemService.DeleteItem(itemId, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<DeleteItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Deletes an item.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
