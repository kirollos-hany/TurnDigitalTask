using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Validations.Validators;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategory>
{
    public CreateCategoryValidator(IReadRepository repository)
    {
        RuleFor(cmd => cmd.Name)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired)
            .Matches(Constants.ValidationRegex.StringNoSymbols)
            .WithMessage(Constants.ValidationMessages.Messages.NameFormat)
            .DependentRules(() =>
            {
                RuleFor(cmd => cmd.Name)
                    .MustAsync(async (name, cancellationToken) => !await repository
                        .GetQueryable<Domain.Features.Categories.Entities.Category>().ByName(name)
                        .AnyAsync(cancellationToken))
                    .WithMessage(Constants.ValidationMessages.Messages.NameExists);
            });
    }
}