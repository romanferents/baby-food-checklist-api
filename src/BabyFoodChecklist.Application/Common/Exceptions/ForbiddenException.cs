namespace BabyFoodChecklist.Application.Common.Exceptions;

/// <summary>
/// Thrown when an operation is not allowed on a resource (e.g. trying to delete a default product).
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ForbiddenException"/> with a message.
    /// </summary>
    public ForbiddenException(string message) : base(message)
    {
    }
}
