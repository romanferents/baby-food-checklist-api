using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IApplicationDbContext context, IAuthService authService)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower(), cancellationToken);

        if (user is null || !authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new ForbiddenException("Invalid username or password.");
        }

        var token = authService.GenerateToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = token,
        };
    }
}
