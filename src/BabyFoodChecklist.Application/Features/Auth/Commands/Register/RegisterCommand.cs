using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthResponseDto>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
