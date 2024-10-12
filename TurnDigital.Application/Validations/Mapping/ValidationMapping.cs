using FluentValidation.Results;
using TurnDigital.Application.Responses;

namespace TurnDigital.Application.Validations.Mapping;

internal static class ValidationMapping
{
    public static FailureResponse ToFailureResponse(this ValidationResult validationResult)
    {
        var errorDict = validationResult.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessage);
        var responseData = new ValidationFailureResponse(errorDict);
        return FailureResponse<ValidationFailureResponse>.ValidationFailure(responseData);
    }
}