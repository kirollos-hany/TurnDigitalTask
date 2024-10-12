using LanguageExt;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Handlers;

internal sealed class DeleteCategoryHandler : ICommandHandler<DeleteCategory, Either<FailureResponse, Unit>>
{
    private readonly IRepository _repository;

    public DeleteCategoryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<FailureResponse, Unit>> Handle(DeleteCategory request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetEntitySet<Category>().WithProducts().ById(request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Category),
                Constants.ValidationMessages.CategoryMessages.CategoryNotFound));
        }

        _repository.GetEntitySet<Category>().Remove(category);

        await _repository.SaveChangesAsync(cancellationToken);

        return new Unit();
    }
}