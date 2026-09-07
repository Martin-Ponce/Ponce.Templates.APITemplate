using Ponce.Templates.APITemplate.Domain.Entities.Base;
using Ponce.Templates.APITemplate.Domain.Validators.Entities;

namespace Ponce.Templates.APITemplate.Domain.Entities.Sample;

/// <summary>
/// Sample aggregate root demonstrating the entity pattern used across the template: private
/// setters, invariants enforced in the constructor/mutation methods, and validation delegated
/// to entity-specific validators. Delete this once you add your real domain entities.
/// </summary>
public class Item : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool Enabled { get; private set; }
    public DateTime RegisteredAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Item() { } // Required by EF Core

    public Item(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name.ValidateName();
        Description = description.ValidateDescription();
        Enabled = true;
        RegisteredAtUtc = DateTime.UtcNow;
    }

    public void Update(string name, string? description)
    {
        Name = name.ValidateName();
        Description = description.ValidateDescription();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Toggle()
    {
        Enabled = !Enabled;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
