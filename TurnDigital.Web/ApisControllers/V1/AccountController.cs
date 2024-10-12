using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TurnDigital.Application.Features.Account.Responses;
using TurnDigital.Application.Responses;
using TurnDigital.Web.Features.Account.Requests;
using TurnDigital.Web.Mappers;

namespace TurnDigital.Web.ApisControllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationFailureResponse))]
    [SwaggerOperation(
        Summary = "User login by email and password",
        Description = "User login by email and password",
        OperationId = "Accounts.Login",
        Tags = new[] { "Accounts Endpoints" })]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = request.Adapt<Application.Features.Account.Commands.Login>();

        var result = await _mediator.Send(command);

        return result.Match(Ok, failure => failure.ToResponse());
    }
}