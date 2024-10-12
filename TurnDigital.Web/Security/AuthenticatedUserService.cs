using TurnDigital.Domain.Security.Interfaces;

namespace TurnDigital.Web.Security;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly HttpContext? _httpContext;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public int GetId()
    {
        if (_httpContext is null)
            return default;

        var isAuthenticated = _httpContext.User.Identity is not null && _httpContext.User.Identity.IsAuthenticated;
        
        return isAuthenticated ? _httpContext.User.GetId() : default;
    }
}