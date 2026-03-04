using Domain.Exceptions;

namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an operation conflicts with existing data
/// E.g., duplicate email, unique constraint violations
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message)
        : base(message, "CONFLICT")
    {
    }

    public ConflictException(string entityName, string propertyName, object value)
        : base($"{entityName} with {propertyName} '{value}' already exists.", "CONFLICT")
    {
    }
}
