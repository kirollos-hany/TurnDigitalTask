using FluentValidation;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Products.Commands.V1;
using TurnDigital.Application.Validations.Validators;

namespace TurnDigital.Application.Features.Products.Validators.V1;

public class CreateProductV1Validator : AbstractValidator<CreateProductV1>
{
    public CreateProductV1Validator()
    {
        RuleFor(cmd => cmd.Price)
            .ValidatePrice();

        RuleFor(cmd => cmd.Image)
            .ValidateImage();

        RuleFor(cmd => cmd.Name)
            .ValidateRequiredString(Constants.ValidationMessages.Messages.NameRequired);
    }
}