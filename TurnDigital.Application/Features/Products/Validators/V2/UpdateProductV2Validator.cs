using System.Security.Cryptography;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Validations.Validators;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Features.Products.Specifications;

namespace TurnDigital.Application.Features.Products.Validators.V2;

public class UpdateProductV2Validator : AbstractValidator<UpdateProductV2>
{
    public UpdateProductV2Validator(IReadRepository repository)
    {
        RuleFor(cmd => cmd.Price)
            .ValidatePrice()
            .When(cmd => cmd.Price is not null);

        RuleFor(cmd => cmd.Image!)
            .ValidateImage()
            .When(cmd => cmd.Image is not null);

        RuleFor(cmd => cmd.Name!)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired)
            .DependentRules(() =>
            {
                RuleFor(cmd => cmd)
                    .MustAsync(async (cmd, cancellationToken) => !await repository.GetQueryable<Product>()
                        .ByNameAndSelfCategoryIdExcludeId(cmd.Name, cmd.Id).AnyAsync(cancellationToken))
                    .WithName(cmd => nameof(cmd.Name))
                    .WithMessage(Constants.ValidationMessages.Messages.NameExists);
            })
            .When(cmd => cmd.Name is not null);
    }
}