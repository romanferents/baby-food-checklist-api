namespace BabyFoodChecklist.Application.DTOs;

public record AuthResponseDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
