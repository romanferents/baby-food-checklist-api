using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Application.DTOs;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IApplicationDbContext context, IAuthService authService)
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var usernameExists = await context.Users
            .AnyAsync(u => u.Username.ToLower() == request.Username.ToLower(), cancellationToken);

        if (usernameExists)
        {
            throw new Application.Common.Exceptions.ValidationException(
                new[] { new ValidationFailure("Username", "Username is already taken.") });
        }

        var emailExists = await context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
        {
            throw new Application.Common.Exceptions.ValidationException(
                new[] { new ValidationFailure("Email", "Email is already registered.") });
        }

        var user = new ApplicationUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = authService.HashPassword(request.Password),
        };

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

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
