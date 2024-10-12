using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TurnDigital.Application.Features.Account.Commands;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Responses.Enums;
using TurnDigital.Web.Features.Account.Requests;
using TurnDigital.Web.Mappers;
using TurnDigital.Web.Utilities;

namespace TurnDigital.Web.SsrControllers;

public class UsersController : Controller
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Login()
    {
        var isAuthenticated = HttpContext.User.IsAuthenticated();

        if (!isAuthenticated)
        {
            return View();
        }

        return RedirectToAction("Index", "Products");
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        var command = request.Adapt<Login>();

        var result = await _mediator.Send(command);

        return result.Match(_ => RedirectToAction("Index", "Products"), failure =>
        {
            if (failure.State != State.ValidationFailure)
            {
                return failure.ToResponse();
            }

            var validationFailure = failure.GetResponseData() as ValidationFailureResponse;

            ModelState.SetModelStateErrors(validationFailure!);

            return View();
        });
    }

    public async Task<IActionResult> LogOut()
    {
        var command = new SignOutCookieAuthentication();

        await _mediator.Send(command);

        return RedirectToAction("Login");
    }
}