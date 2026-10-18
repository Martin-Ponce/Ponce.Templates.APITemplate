using Ponce.Templates.APITemplate.Domain.Entities.Base;
using Ponce.Templates.APITemplate.Domain.Validators.Entities;

namespace Ponce.Templates.APITemplate.Domain.Entities.Example;

/// <summary>Example aggregate root, rules are enforced in the constructor.</summary>
public class ExampleItem : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private ExampleItem() { }

    public ExampleItem(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name.ValidateName();
        Description = description.ValidateDescription();
    }
}
