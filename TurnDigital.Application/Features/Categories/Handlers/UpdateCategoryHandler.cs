using FluentValidation;
using LanguageExt;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Handlers;

internal sealed class UpdateCategoryHandler : ICommandHandler<UpdateCategory, Either<FailureResponse, CategoryDto>>
{
    private readonly IRepository _repository;

    private readonly IValidator<UpdateCategory> _validator;

    public UpdateCategoryHandler(IRepository repository, IValidator<UpdateCategory> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, CategoryDto>> Handle(UpdateCategory request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResponse();
        }
        
        var category = await _repository.GetEntitySet<Category>().ById(request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return FailureResponse<NotFoundResponse>.NotFound(new NotFoundResponse(nameof(Category),
                Constants.ValidationMessages.CategoryMessages.CategoryNotFound));
        }

        if (category.Name == request.Name)
        {
            return category.Adapt<CategoryDto>();
        }

        category.Name = request.Name;

        await _repository.SaveChangesAsync(cancellationToken);

        return category.Adapt<CategoryDto>();
    }
}