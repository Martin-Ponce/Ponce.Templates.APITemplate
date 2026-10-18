namespace Ponce.Templates.APITemplate.Domain.Entities.Base;

/// <summary>Base for aggregate roots, the derived class sets the id.</summary>
public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }

    protected AggregateRoot()
    {
        Id = Guid.Empty;
    }
}
