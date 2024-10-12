using Microsoft.AspNetCore.Mvc;

namespace TurnDigital.Web.Common.Requests;

public class PaginationRequest
{
    private int _pageIndex = 1;

    [FromQuery]
    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    private int _pageCount = 25;

    [FromQuery]
    public int PageCount
    {
        get => _pageCount;
        set => _pageCount = value < 1 || value > 100 ? 25 : value;
    }
}