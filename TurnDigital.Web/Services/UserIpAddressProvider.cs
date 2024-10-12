using System.Net;
using TurnDigital.Domain.Security.Interfaces;
using TurnDigital.Web.Configurations;

namespace TurnDigital.Web.Services;

public class UserIpAddressProvider : IUserIpAddressProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ProxyConfig _proxyConfig;

    public UserIpAddressProvider(IHttpContextAccessor httpContextAccessor, ProxyConfig proxyConfig)
    {
        _httpContextAccessor = httpContextAccessor;
        _proxyConfig = proxyConfig;
    }

    public string GetIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return string.Empty;

        if (httpContext.Request.Headers.ContainsKey(_proxyConfig.ForwardHeaderName))
            return httpContext.Request.Headers[_proxyConfig.ForwardHeaderName].ToString();
        
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
        {
            ipAddress = GetLocalIpAddress();
        }

        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return httpContext.Request.Headers["X-Forwarded-For"].ToString();
        }

        if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
        {
            return httpContext.Request.Headers["X-Real-IP"].ToString();
        }

        return ipAddress ?? string.Empty;
    }
    
    private static string? GetLocalIpAddress()
    {
        var ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        return ipAddresses
            .Where(ipAddress => ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .Select(ipAddress => ipAddress.ToString())
            .FirstOrDefault();
    }
}