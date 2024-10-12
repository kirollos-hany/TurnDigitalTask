using System.Text;
using TurnDigital.Domain.Web.Interfaces;

namespace TurnDigital.Web.Services;

public class BaseUrlProvider : IBaseUrlProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    private string _url = string.Empty;

    public BaseUrlProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetBaseUrl()
    {
        if (_url != string.Empty)
        {
            return _url;
        }
        _url = new StringBuilder(_contextAccessor.HttpContext!.Request.Scheme).Append("://").Append(_contextAccessor.HttpContext!.Request.Host).Append(_contextAccessor.HttpContext!.Request.PathBase.ToString()).ToString();
        return _url;
    }
}