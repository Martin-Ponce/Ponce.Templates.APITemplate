namespace Ponce.Templates.APITemplate.API.Endpoints.Items;

/// <summary>
/// Sample endpoint group demonstrating the minimal-API pattern used across the template: one
/// static class per endpoint group that maps a route group, and one static class per endpoint.
/// Copy this shape for every real feature, then delete this sample.
/// </summary>
public static class ItemEndpointGroup
{
    public static RouteGroupBuilder MapItemEndpoints(this RouteGroupBuilder appEndpoints)
    {
        var itemEndpointGroup = appEndpoints.MapGroup("items").WithTags("Items");
        itemEndpointGroup
            .MapGetItems()
            .MapGetItem()
            .MapSaveItem()
            .MapUpdateItem()
            .MapToggleItem()
            .MapDeleteItem();
        return appEndpoints;
    }
}
