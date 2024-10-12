using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Validations.Validators;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;

namespace TurnDigital.Application.Features.Products.Validators.V2;

public class CreateProductV2Validator : AbstractValidator<CreateProductV2>
{
    public CreateProductV2Validator(IReadRepository repository)
    {
        RuleFor(cmd => cmd.Price)
            .ValidatePrice();

        RuleFor(cmd => cmd.Image)
            .ValidateImage();

        RuleFor(cmd => cmd.Name)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired)
            .DependentRules(() =>
            {
                RuleFor(cmd => cmd)
                    .MustAsync(async (cmd, cancellationToken) => !await repository.GetQueryable<Product>()
                        .ByName(cmd.Name).AnyAsync(cancellationToken))
                    .WithName(cmd => nameof(cmd.Name))
                    .WithMessage(Constants.ValidationMessages.Messages.NameExists);
            });

        RuleFor(cmd => cmd.CategoryId)
            .MustAsync(async (categoryId, cancellationToken) =>
                await repository.GetQueryable<Category>().ById(categoryId).AnyAsync(cancellationToken))
            .WithMessage(Constants.ValidationMessages.CategoryMessages.CategoryNotFound);
    }
}