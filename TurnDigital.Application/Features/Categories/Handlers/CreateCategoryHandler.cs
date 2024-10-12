using FluentValidation;
using LanguageExt;
using Mapster;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Categories.Entities;

namespace TurnDigital.Application.Features.Categories.Handlers;

internal sealed class CreateCategoryHandler : ICommandHandler<CreateCategory, Either<FailureResponse, CategoryDto>>
{
    private readonly IRepository _repository;

    private readonly IValidator<CreateCategory> _validator;

    public CreateCategoryHandler(IRepository repository, IValidator<CreateCategory> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, CategoryDto>> Handle(CreateCategory request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResponse();
        }
        
        var category = new Category
        {
            Name = request.Name
        };

        _repository.GetEntitySet<Category>().Add(category);

        await _repository.SaveChangesAsync(cancellationToken);

        return category.Adapt<CategoryDto>();
    }
}