namespace Ponce.Templates.APITemplate.Application.Dtos.Response.Items;

public class GetItemResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
