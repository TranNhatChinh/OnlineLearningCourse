using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class UserProfile
{
    public Guid Id { get; private set; }
    public string? FullName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Phone { get; private set; }
    public UserStatus Status { get; private set; }
    public UserRole UserRole { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(
        Guid userId,
        UserRole role,
        string? fullName = null,
        string? avatarUrl = null,
        string? phone = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required", "USER_ID_REQUIRED");

        return new UserProfile
        {
            Id = userId,
            UserRole = role,
            Status = UserStatus.Active,
            FullName = fullName,
            AvatarUrl = avatarUrl,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string? fullName, string? avatarUrl, string? phone)
    {
        FullName = fullName;
        AvatarUrl = avatarUrl;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(UserStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
