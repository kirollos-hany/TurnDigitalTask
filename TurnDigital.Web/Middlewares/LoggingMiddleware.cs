using TurnDigital.Web.Security;
using TurnDigital.Web.Utilities;

namespace TurnDigital.Web.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        var isAuthenticated = context.User.IsAuthenticated();

        var userId = isAuthenticated ? context.User.GetId().ToString() : "Anonymous";

        var method = context.Request.Method;

        var endpoint = context.Request.Path.ToString();

        _logger.LogInformation("User: {userId} invoked a {method} request to {endpoint} with response of status {statusCode}", userId, method, endpoint, context.Response.StatusCode);
    }
}