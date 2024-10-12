using System.Security.Claims;

namespace TurnDigital.Domain.Security.Interfaces;

public interface IJwtProvider
{
    (string token, DateTime expiration) GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
}