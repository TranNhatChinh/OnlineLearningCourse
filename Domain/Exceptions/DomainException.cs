namespace Domain.Exceptions;

/// <summary>
/// Base exception for domain invariant violations.
/// Lives in Domain so entities can throw without depending on Application.
/// </summary>
public class DomainException : Exception
{
    public string? ErrorCode { get; }

    public DomainException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string message, Exception innerException, string? errorCode = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
