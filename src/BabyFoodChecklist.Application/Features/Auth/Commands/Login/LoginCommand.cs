using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthResponseDto>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
