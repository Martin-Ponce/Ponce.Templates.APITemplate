namespace Ponce.Templates.APITemplate.Domain.Entities.Base;

/// <summary>
/// Base class for aggregate roots. Unlike <see cref="Entity"/>, the identifier is left to the
/// derived class (either a generated Guid assigned in its constructor, or a natural/business key).
/// </summary>
public abstract class AggregateRoot
{
    // Add navigation properties on the entities that belong to this aggregate root as needed,
    // defaulting them with "= default!" since they are populated by EF Core.
    public Guid Id { get; protected set; }

    protected AggregateRoot()
    {
        Id = Guid.Empty;
    }
}
