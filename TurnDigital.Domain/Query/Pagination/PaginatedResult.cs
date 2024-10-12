namespace TurnDigital.Domain.Query.Pagination;

public record PaginatedResult<T>(int Count, IReadOnlyList<T> Items, int PageIndex)
{
    public bool HasNextPage => PageIndex < PagesCount;

    public bool HasPreviousPage => PageIndex > 1;

    public int PagesCount => Items.Count == 0 ? 0 : Count / Items.Count;
}