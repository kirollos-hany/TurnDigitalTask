namespace TurnDigital.Application.Responses;

public class MessageResponse
{
    public string Message { get; }

    public MessageResponse(string message)
    {
        Message = message;
    }
}