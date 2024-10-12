namespace TurnDigital.Domain.ValueObjects;


public record RefreshToken(string Token)
{
    public string Token { get; private set; } = Token;
    public DateTime RevokedAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    internal void Revoke(string token)
    {
        Token = token;
        RevokedAt = DateTime.UtcNow;
    }
}

