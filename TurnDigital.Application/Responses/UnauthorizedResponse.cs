namespace TurnDigital.Application.Responses;

public class UnauthorizedResponse
{
    public UnauthorizedResponse(UnauthorizedReason reason)
    {
        Reason = reason;
    }
    
    public UnauthorizedReason Reason { get; }
}

public enum UnauthorizedReason
{
    InvalidAccessToken,
    
    AccountDeactivated,
    
    NotHavePermissions
}