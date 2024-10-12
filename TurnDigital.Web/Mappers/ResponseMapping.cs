using Microsoft.AspNetCore.Mvc;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Responses.Enums;

namespace TurnDigital.Web.Mappers;

public static class ResponseMapping
{
    /// <summary>
    /// Maps failure response to the appropriate http response.
    /// </summary>
    /// <returns>An action result corresponding to the appropriate response.</returns>
    public static ActionResult ToResponse(this FailureResponse failureResponse)
        {
            return failureResponse.State switch
            {
                State.Unauthorized => new UnauthorizedObjectResult(failureResponse.GetResponseData()),
                State.ValidationFailure => new BadRequestObjectResult(failureResponse.GetResponseData()),
                State.NotFound => new NotFoundObjectResult(failureResponse.GetResponseData()),
                State.InternalError => new ObjectResult(failureResponse.GetResponseData()){StatusCode = StatusCodes.Status500InternalServerError},
                State.TooManyRequests => new ObjectResult(failureResponse.GetResponseData()){StatusCode = StatusCodes.Status429TooManyRequests},
                _ => throw new ArgumentException("Response state not supported.")
            };
        }
}
