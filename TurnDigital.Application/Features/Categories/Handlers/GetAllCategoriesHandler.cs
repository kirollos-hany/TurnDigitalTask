using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Categories.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Handlers;

internal sealed class GetAllCategoriesHandler : IQueryHandler<GetAllCategories, CollectionWrapper<CategoryDto>>
{
    private readonly IReadRepository _repository;

    public GetAllCategoriesHandler(IReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<CollectionWrapper<CategoryDto>> Handle(GetAllCategories request,
        CancellationToken cancellationToken)
    {
        var results = await _repository.GetQueryable<Category>().OrderByNewest().ToDto().ToListAsync(cancellationToken);

        return new CollectionWrapper<CategoryDto>
        {
            Results = results
        };
    }
}