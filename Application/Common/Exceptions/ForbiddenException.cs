using Domain.Exceptions;

namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user lacks permission for an operation
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You don't have permission to perform this action.")
        : base(message, "FORBIDDEN")
    {
    }
}
