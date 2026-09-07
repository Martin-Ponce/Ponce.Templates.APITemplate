namespace Ponce.Templates.APITemplate.Domain.Entities.Base;

/// <summary>
/// Base class for entities that are not the root of an aggregate (e.g. owned/child entities).
/// The identifier is generated automatically on creation.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}
