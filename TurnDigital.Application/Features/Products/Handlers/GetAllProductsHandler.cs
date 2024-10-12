using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Queries;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;
using TurnDigital.Domain.Query.Pagination;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Application.Features.Products.Handlers;

internal sealed class GetAllProductsHandler : IQueryHandler<GetAllProducts, PaginatedResult<ProductDto>>
{
    private readonly IReadRepository _repository;

    public GetAllProductsHandler(IReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProducts request, CancellationToken cancellationToken)
    {
        var queryable = _repository.GetQueryable<Product>();

        if (!string.IsNullOrEmpty(request.Name))
        {
            queryable = queryable.NameIncludes(request.Name);
        }

        if (request.CategoryId is not null)
        {
            queryable = queryable.ByCategoryId(request.CategoryId.Value);
        }

        var results = await queryable.OrderByNewest().Paginate(request.PaginationParameters).ToDto().ToListAsync(cancellationToken);

        var count = await queryable.CountAsync(cancellationToken);

        return new PaginatedResult<ProductDto>(count, results, request.PaginationParameters.PageIndex);
    }
}