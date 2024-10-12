using TurnDigital.Domain.Query.Pagination;

namespace TurnDigital.Domain.Utilities;

public static class QueryableUtilities
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationParameters paginationParameters) =>
        queryable.Skip((paginationParameters.PageIndex - 1) * paginationParameters.PageCount)
            .Take(paginationParameters.PageCount);
}