namespace BabyFoodChecklist.McpServer.Tools;

internal static class UserIdParser
{
    /// <summary>
    /// Parses and validates a userId string as a GUID.
    /// Returns the parsed GUID or throws an error message string if invalid.
    /// </summary>
    public static (bool IsValid, Guid UserId, string? Error) Parse(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return (false, Guid.Empty, "Error: userId parameter is required. Please provide a valid user ID (GUID).");
        }

        if (!Guid.TryParse(userId, out var parsed) || parsed == Guid.Empty)
        {
            return (false, Guid.Empty, $"Error: \"{userId}\" is not a valid user ID. Please provide a valid GUID (e.g., \"a1b2c3d4-e5f6-7890-abcd-ef1234567890\").");
        }

        return (true, parsed, null);
    }
}
