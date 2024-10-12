using Microsoft.AspNetCore.Diagnostics;
using TurnDigital.Application.Responses;
using Constants = TurnDigital.Application.Common.Constants;

namespace TurnDigital.Web.Middlewares;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        const string message = Constants.ResponseMessages.InternalServerErrorMessage;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var messageResponse = new MessageResponse(message);

        await httpContext.Response.WriteAsJsonAsync(messageResponse, cancellationToken);
        
        return true;
    }
}