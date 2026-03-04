using Domain.Exceptions;

namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.", "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    {
    }
}
