namespace Ponce.Templates.APITemplate.Domain.Entities.Base;

/// <summary>Base for child entities, generates its own id.</summary>
public abstract class Entity
{
    public Guid Id { get; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}
