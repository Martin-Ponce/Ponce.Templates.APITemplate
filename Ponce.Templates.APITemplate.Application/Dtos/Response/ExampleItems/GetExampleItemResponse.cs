namespace Ponce.Templates.APITemplate.Application.Dtos.Response.ExampleItems;

public class GetExampleItemResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
