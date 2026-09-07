namespace Ponce.Templates.APITemplate.Application.Dtos.Response.Items;

public class SaveItemResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
