using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Categories.Commands;
using TurnDigital.Application.Features.Categories.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Responses.Enums;
using TurnDigital.Domain.Security.Enums;
using TurnDigital.Web.Features.Categories.Requests;
using TurnDigital.Web.Mappers;
using TurnDigital.Web.Utilities;

namespace TurnDigital.Web.SsrControllers;

[Authorize(Roles = nameof(Roles.TurnDigitalAdmin))]
public class CategoriesController : Controller
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var query = new GetAllCategories();

        var categories = await _mediator.Send(query);

        return View(categories);
    }
    
    public IActionResult AddCategory()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory([FromForm] AddCategoryRequest request)
    {
        var command = request.Adapt<CreateCategory>();

        var result = await _mediator.Send(command);

        return result.Match(dto =>
        {
            ViewData[Constants.ViewDataKeys.Message] = Constants.Messages.CreateSuccessful;

            return View();
        }, failure =>
        {
            if (failure.State == State.ValidationFailure)
            {
                ModelState.SetModelStateErrors((failure.GetResponseData() as ValidationFailureResponse)!);
            }

            return View();
        });
    }

    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        var command = new DeleteCategory(id);

        var result = await _mediator.Send(command);

        return result.Match(_ => NoContent(), failure => failure.ToResponse());
    }

    [HttpGet("categories/{id:int}")]
    public async Task<IActionResult> CategoryDetails([FromRoute] int id)
    {
        var query = new GetCategoryById(id);

        var result = await _mediator.Send(query);

        return result.Match(dto => View(dto.Adapt<EditCategoryRequest>()), failure => failure.ToResponse());
    }

    [HttpPost("categories/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory([FromRoute] int id, [FromForm] EditCategoryRequest request)
    {
        var command = request.Adapt<UpdateCategory>();

        command = command with { Id = id };

        var result = await _mediator.Send(command);

        return result.Match(dto =>
        {
            ViewData[Constants.ViewDataKeys.Message] = Constants.Messages.UpdateSuccessful;

            return View("CategoryDetails", dto.Adapt<EditCategoryRequest>());
        }, failure =>
        {
            if (failure.State == State.ValidationFailure)
            {
                ModelState.SetModelStateErrors((failure.GetResponseData() as ValidationFailureResponse)!);
            }

            return View("CategoryDetails");
        });
    }
}