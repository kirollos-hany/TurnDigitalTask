using System.Security.Claims;

namespace TurnDigital.Domain.Security.Interfaces;

public interface ICookieAuthenticationService
{
    Task SignInAsync(IEnumerable<Claim> claims);

    Task SignOutAsync();
}