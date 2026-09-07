namespace Ponce.Templates.APITemplate.Application.Dtos.Response.Items;

public class UpdateItemResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
