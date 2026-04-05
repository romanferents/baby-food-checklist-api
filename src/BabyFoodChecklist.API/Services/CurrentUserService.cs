using System.Security.Claims;
using BabyFoodChecklist.Application.Common.Interfaces;

namespace BabyFoodChecklist.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim is not null && Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
