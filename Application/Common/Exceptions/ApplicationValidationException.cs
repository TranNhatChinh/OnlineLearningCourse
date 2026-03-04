using FluentValidation.Results;
using Domain.Exceptions;

namespace Application.Common.Exceptions;

public class ApplicationValidationException : DomainException
{
    public IDictionary<string, string[]>? Errors { get; }

    public ApplicationValidationException(string message)
        : base(message, "VALIDATION_ERROR") { }

    public ApplicationValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.", "VALIDATION_ERROR")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );
    }
}
