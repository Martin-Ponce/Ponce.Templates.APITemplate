using Ponce.Templates.APITemplate.Domain.Entities.Base;
using Ponce.Templates.APITemplate.Domain.Validators.Entities;

namespace Ponce.Templates.APITemplate.Domain.Entities.Example;

/// <summary>Aggregate root for the "ExampleItem" feature: private setters, invariants enforced in the constructor via <see cref="ExampleItemValidator"/>.</summary>
public class ExampleItem : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private ExampleItem() { } // Required by EF Core

    public ExampleItem(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name.ValidateName();
        Description = description.ValidateDescription();
    }
}
