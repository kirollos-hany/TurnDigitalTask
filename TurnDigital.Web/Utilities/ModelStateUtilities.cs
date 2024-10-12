using Microsoft.AspNetCore.Mvc.ModelBinding;
using TurnDigital.Application.Responses;

namespace TurnDigital.Web.Utilities;

public static class ModelStateUtilities
{
    public static void SetModelStateErrors(this ModelStateDictionary modelState,
        ValidationFailureResponse failureResponse)
    {
        var errorDict = failureResponse.ValidationMap;

        foreach (var pair in errorDict)
        {
            modelState.AddModelError(pair.Key, pair.Value);
        }
    }
}