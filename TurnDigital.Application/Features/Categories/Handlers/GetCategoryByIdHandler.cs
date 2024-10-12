using LanguageExt;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Categories.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Handlers;

public class GetCategoryByIdHandler : IQueryHandler<GetCategoryById, Either<FailureResponse, CategoryDto>>
{
    private readonly IReadRepository _repository;

    public GetCategoryByIdHandler(IReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<FailureResponse, CategoryDto>> Handle(GetCategoryById request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetQueryable<Category>().ById(request.Id).ToDto()
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Category),
                Constants.ValidationMessages.CategoryMessages.CategoryNotFound));
        }

        return category;
    }
}