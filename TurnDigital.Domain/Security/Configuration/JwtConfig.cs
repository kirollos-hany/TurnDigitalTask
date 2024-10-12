namespace TurnDigital.Domain.Security.Configuration;

public class JwtConfig
{
    public string Secrets { get; set; } = string.Empty;
    
    public string Issuer { get; set; } = string.Empty;
    
    public string Audience { get; set; } = string.Empty;

    public int AccessTokenExpirationInMinutes { get; set; }
}