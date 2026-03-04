using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class InstructorProfile
{
    public Guid Id { get; private set; }
    public string? Bio { get; private set; }
    public string? Expertise { get; private set; }
    public InstructorVerificationStatus VerificationStatus { get; private set; }
    public decimal RatingAvg { get; private set; }
    public int RatingCount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private InstructorProfile() { }

    public static InstructorProfile Create(Guid instructorId, string? bio = null, string? expertise = null)
    {
        if (instructorId == Guid.Empty)
            throw new DomainException("Instructor id is required", "INSTRUCTOR_ID_REQUIRED");

        return new InstructorProfile
        {
            Id = instructorId,
            Bio = bio,
            Expertise = expertise,
            VerificationStatus = InstructorVerificationStatus.Pending,
            RatingAvg = 0m,
            RatingCount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string? bio, string? expertise)
    {
        Bio = bio;
        Expertise = expertise;
    }

    public void SetVerificationStatus(InstructorVerificationStatus status)
    {
        VerificationStatus = status;
    }

    public void ApplyRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5", "RATING_INVALID");

        var total = (RatingAvg * RatingCount) + rating;
        RatingCount += 1;
        RatingAvg = total / RatingCount;
    }
}
