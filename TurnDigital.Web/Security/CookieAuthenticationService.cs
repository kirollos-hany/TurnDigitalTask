using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TurnDigital.Application.Common;
using TurnDigital.Domain.Security.Interfaces;

namespace TurnDigital.Web.Security;

public class CookieAuthenticationService : ICookieAuthenticationService
{
    private readonly HttpContext? _httpContext;

    public CookieAuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task SignInAsync(IEnumerable<Claim> claims)
    {
        if (_httpContext is null)
        {
            return;
        }

        var claimsIdentity = new ClaimsIdentity(claims, Constants.AuthenticationSchemes.JwtOrCookies);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // if you want to keep the cookie across browser sessions
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await _httpContext.SignInAsync(
            Constants.AuthenticationSchemes.JwtOrCookies,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    public async Task SignOutAsync()
    {
        if (_httpContext is null)
        {
            return;
        }

        await _httpContext.SignOutAsync();
    }
}