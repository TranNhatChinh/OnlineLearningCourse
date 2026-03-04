using Domain.Exceptions;

namespace Domain.Entities;

public class ParentProfile
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ParentProfile() { }

    public static ParentProfile Create(Guid parentId)
    {
        if (parentId == Guid.Empty)
            throw new DomainException("Parent id is required", "PARENT_ID_REQUIRED");

        return new ParentProfile
        {
            Id = parentId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
