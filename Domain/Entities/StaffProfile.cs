using Domain.Exceptions;

namespace Domain.Entities;

public class StaffProfile
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private StaffProfile() { }

    public static StaffProfile Create(Guid staffId)
    {
        if (staffId == Guid.Empty)
            throw new DomainException("Staff id is required", "STAFF_ID_REQUIRED");

        return new StaffProfile
        {
            Id = staffId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
