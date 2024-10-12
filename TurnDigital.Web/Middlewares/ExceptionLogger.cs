using Microsoft.AspNetCore.Diagnostics;
using TurnDigital.Web.Security;
using TurnDigital.Web.Utilities;

namespace TurnDigital.Web.Middlewares;

public class ExceptionLogger : IExceptionHandler
{
    private readonly ILogger<ExceptionLogger> _logger;

    public ExceptionLogger(ILogger<ExceptionLogger> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var isAuthenticated = httpContext.User.IsAuthenticated();

        var userId = isAuthenticated ? httpContext.User.GetId().ToString() : "Anonymous";

        var method = httpContext.Request.Method;

        var endpoint = httpContext.Request.Path.ToString();

        _logger.LogError(exception, "User: {userId} invoked a {method} request to {endpoint} with a result of an exception", userId, method, endpoint);
        
        return ValueTask.FromResult(false);
    }
}