namespace BabyFoodChecklist.Application.Common.Exceptions;

/// <summary>
/// Thrown when a validation rule fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors keyed by field name.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/> from FluentValidation failures.
    /// </summary>
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
