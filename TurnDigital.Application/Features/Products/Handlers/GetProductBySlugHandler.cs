using LanguageExt;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Products.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;

namespace TurnDigital.Application.Features.Products.Handlers;

internal sealed class GetProductBySlugHandler : IQueryHandler<GetProductBySlug, Either<FailureResponse, ProductDto>>
{
    private readonly IReadRepository _repository;

    public GetProductBySlugHandler(IReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<FailureResponse, ProductDto>> Handle(GetProductBySlug request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetQueryable<Product>().BySlug(request.Slug).ToDto()
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Product),
                Constants.ValidationMessages.ProductMessages.ProductNotFound));
        }

        return product;
    }
}