using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Request.ExampleItems;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class SaveExampleItem
{
    public const string ENDPOINT_NAME = "Create example item";

    public static RouteGroupBuilder MapSaveExampleItem(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapPost("",
            async ([FromBody] SaveExampleItemRequest request, ExampleItemService exampleItemService, CancellationToken ct) =>
            {
                var response = await exampleItemService.SaveExampleItem(request, ct);
                return Results.Created($"exampleitems/{response.Id}", response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<SaveExampleItemResponse>(StatusCodes.Status201Created)
        .WithDescription("Creates a new example item.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
