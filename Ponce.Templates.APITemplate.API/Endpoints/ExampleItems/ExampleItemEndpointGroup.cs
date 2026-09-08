namespace Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;

/// <summary>
/// Endpoint group demonstrating the minimal-API pattern used across the template: one static class
/// per endpoint group that maps a route group, and one static class per endpoint.
/// </summary>
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
