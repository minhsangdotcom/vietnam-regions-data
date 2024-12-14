namespace VnRegion.Common;

public abstract class BaseEntity
{
    public Ulid Id { get; protected set; } = Ulid.NewUlid();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public abstract class BaseEntity<T>
{
    public virtual T Id { get; protected set; } = default!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
