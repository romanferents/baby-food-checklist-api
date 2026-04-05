using BabyFoodChecklist.Domain.Entities;

namespace BabyFoodChecklist.Application.Common.Interfaces;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string GenerateToken(ApplicationUser user);
}
