namespace TurnDigital.Application.Responses;

public class NotFoundResponse
{
    public NotFoundResponse(string resourceName, string message)
    {
        ResourceName = resourceName;

        Message = message;
    }
    
    public string ResourceName { get; }
    
    public string Message { get; }
}