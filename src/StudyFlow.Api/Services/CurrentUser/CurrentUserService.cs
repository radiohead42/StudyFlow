using System.Security.Claims;

namespace StudyFlow.Api.Services.CurrentUser;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor): ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new InvalidOperationException("User is not authenticated.");
}
