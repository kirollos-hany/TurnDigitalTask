using FluentValidation;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Products.Commands.V1;
using TurnDigital.Application.Validations.Validators;

namespace TurnDigital.Application.Features.Products.Validators.V1;

public class UpdateProductV1Validator : AbstractValidator<UpdateProductV1>
{
    public UpdateProductV1Validator()
    {
        RuleFor(cmd => cmd.Price)
            .ValidatePrice();

        RuleFor(cmd => cmd.Image!)
            .ValidateImage()
            .When(cmd => cmd.Image is not null);

        RuleFor(cmd => cmd.Name)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired);
    }
}