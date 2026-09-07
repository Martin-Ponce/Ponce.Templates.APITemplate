namespace Ponce.Templates.APITemplate.Application.Dtos.Request.Items;

public class UpdateItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
