namespace TurnDigital.Domain.Query.Pagination;

public class PaginationParameters
{
    public PaginationParameters(int pageCount, int pageIndex = 1)
    {
        PageCount = pageCount;
        PageIndex = pageIndex;
    }
    
    public int PageIndex { get; } 
    
    public int PageCount { get; }
}