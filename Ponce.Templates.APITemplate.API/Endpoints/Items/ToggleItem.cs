using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class ToggleItem
{
    public const string ENDPOINT_NAME = "Toggle item";

    public static RouteGroupBuilder MapToggleItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapPatch("{itemId:guid}/toggle",
            async ([FromRoute] Guid itemId, ItemService itemService, CancellationToken ct) =>
            {
                var response = await itemService.ToggleItem(itemId, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<ToggleItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Enables or disables an item.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
