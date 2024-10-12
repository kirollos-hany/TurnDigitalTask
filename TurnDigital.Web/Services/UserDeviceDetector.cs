using MyCSharp.HttpUserAgentParser.Providers;
using TurnDigital.Domain.Security.Interfaces;
using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Web.Services;

public class UserDeviceDetector : IUserDeviceDetector
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IHttpUserAgentParserProvider _httpUserAgentParserProvider;

    public UserDeviceDetector(IHttpContextAccessor httpContextAccessor, IHttpUserAgentParserProvider httpUserAgentParserProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpUserAgentParserProvider = httpUserAgentParserProvider;
    }

    public DeviceInfo? DetectDevice()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return default;

        var userAgent = httpContext.Request.Headers["User-Agent"];

        var info = _httpUserAgentParserProvider.Parse(userAgent.ToString());

        return new DeviceInfo(info.Type.ToString(), info.Name ?? string.Empty, info.Platform?.Name ?? string.Empty);
    }

    public string UserAgent()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        return httpContext is null ? string.Empty : httpContext.Request.Headers["User-Agent"].ToString();
    }
}