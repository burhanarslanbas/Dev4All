namespace Dev4All.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; protected set; }
    public DateTime? DeletedDate { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkAsUpdated() => UpdatedDate = DateTime.UtcNow;
    public void MarkAsDeleted() => DeletedDate = DateTime.UtcNow;
}
