using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Request.Items;
using Ponce.Templates.APITemplate.Application.Dtos.Response.Items;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

public static class SaveItem
{
    public const string ENDPOINT_NAME = "Create item";

    public static RouteGroupBuilder MapSaveItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapPost("",
            async ([FromBody] SaveItemRequest request, ItemService itemService, CancellationToken ct) =>
            {
                var response = await itemService.SaveItem(request, ct);
                return Results.Created($"items/{response.Id}", response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<SaveItemResponse>(StatusCodes.Status201Created)
        .WithDescription("Creates a new item.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
