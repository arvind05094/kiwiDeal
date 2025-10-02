namespace kiwiDeal.SharedKernel.Entities;

public abstract class BaseEntity
{
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }
}
