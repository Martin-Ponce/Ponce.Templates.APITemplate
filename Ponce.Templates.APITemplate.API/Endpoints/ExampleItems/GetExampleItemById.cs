using Microsoft.AspNetCore.Mvc;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;
using Ponce.Templates.APITemplate.Application.Services;

namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

public static class GetExampleItemById
{
    public const string ENDPOINT_NAME = "Get example item by id";

    public static RouteGroupBuilder MapGetExampleItemById(this RouteGroupBuilder endpointGroup)
    {
        endpointGroup.MapGet("{exampleItemId:guid}",
            async ([FromRoute] Guid exampleItemId, ExampleItemService exampleItemService, CancellationToken ct) =>
            {
                var response = await exampleItemService.GetExampleItem(exampleItemId, ct);
                return Results.Ok(response);
            }
        )
        .HasApiVersion(ApiConstants.VERSION_1)
        .Produces<GetExampleItemResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithDescription("Gets a specific example item by id.")
        .WithSummary(ENDPOINT_NAME)
        .WithName(ENDPOINT_NAME);
        return endpointGroup;
    }
}
