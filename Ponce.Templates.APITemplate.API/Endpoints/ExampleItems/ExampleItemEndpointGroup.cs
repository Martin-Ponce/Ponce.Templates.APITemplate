namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

/// <summary>Maps the example item endpoints.</summary>
public static class ExampleItemEndpointGroup
{
    public static RouteGroupBuilder MapExampleItemEndpoints(this RouteGroupBuilder appEndpoints)
    {
        var exampleItemEndpointGroup = appEndpoints.MapGroup("exampleitems").WithTags("ExampleItems");
        exampleItemEndpointGroup
            .MapGetExampleItems()
            .MapGetExampleItemById()
            .MapSaveExampleItem();
        return appEndpoints;
    }
}
