using Domain.Exceptions;

namespace Domain.Entities;

public class LearnerProfile
{
    public Guid Id { get; private set; }
    public Guid? ParentId { get; private set; }
    public string? GradeLevel { get; private set; }
    public int? BirthYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private LearnerProfile() { }

    public static LearnerProfile Create(Guid learnerId, Guid? parentId, string? gradeLevel, int? birthYear)
    {
        if (learnerId == Guid.Empty)
            throw new DomainException("Learner id is required", "LEARNER_ID_REQUIRED");

        if (birthYear.HasValue && (birthYear < 1900 || birthYear > 2100))
            throw new DomainException("Birth year is invalid", "BIRTH_YEAR_INVALID");

        return new LearnerProfile
        {
            Id = learnerId,
            ParentId = parentId,
            GradeLevel = gradeLevel,
            BirthYear = birthYear,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AssignParent(Guid parentId)
    {
        if (parentId == Guid.Empty)
            throw new DomainException("Parent id is required", "PARENT_ID_REQUIRED");

        ParentId = parentId;
    }

    public void UpdateProfile(string? gradeLevel, int? birthYear)
    {
        if (birthYear.HasValue && (birthYear < 1900 || birthYear > 2100))
            throw new DomainException("Birth year is invalid", "BIRTH_YEAR_INVALID");

        GradeLevel = gradeLevel;
        BirthYear = birthYear;
    }
}
