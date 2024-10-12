namespace TurnDigital.Application.Features.Account.Responses;

public class LoginResponse
{
    public required string AccessToken { get; init; }
    
    public required string RefreshToken { get; init; }
    
    public required DateTime AccessTokenExpiration { get; init; }
}