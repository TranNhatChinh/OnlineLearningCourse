using Domain.Exceptions;

namespace Domain.Entities;

public class AdminProfile
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AdminProfile() { }

    public static AdminProfile Create(Guid adminId)
    {
        if (adminId == Guid.Empty)
            throw new DomainException("Admin id is required", "ADMIN_ID_REQUIRED");

        return new AdminProfile
        {
            Id = adminId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
