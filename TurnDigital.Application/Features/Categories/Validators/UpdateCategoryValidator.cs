using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Validations.Validators;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Categories.Specifications;

namespace TurnDigital.Application.Features.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategory>
{
    public UpdateCategoryValidator(IReadRepository repository)
    {
        RuleFor(cmd => cmd.Name)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired)
            .Matches(Constants.ValidationRegex.StringNoSymbols)
            .WithMessage(Constants.ValidationMessages.Messages.NameFormat)
            .DependentRules(() =>
            {
                RuleFor(cmd => cmd)
                    .MustAsync(async (cmd, cancellationToken) => !await repository.GetQueryable<Category>()
                        .ByNameExcludeId(cmd.Name, cmd.Id).AnyAsync(cancellationToken))
                    .WithName(cmd => nameof(cmd.Name))
                    .WithMessage(Constants.ValidationMessages.Messages.NameExists);
            });
    }
}