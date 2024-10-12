using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Categories.Queries;
using TurnDigital.Application.Features.Products.Commands;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Features.Products.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Responses.Enums;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Query.Pagination;
using TurnDigital.Domain.Security.Enums;
using TurnDigital.Web.Features.Products.Requests;
using TurnDigital.Web.Features.Products.ViewModels;
using TurnDigital.Web.Mappers;
using TurnDigital.Web.Utilities;

namespace TurnDigital.Web.SsrControllers;

[Authorize(Roles = nameof(Roles.TurnDigitalAdmin))]
public class ProductsController : Controller
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index([AsParameters] GetAllProductsRequest request)
    {
        var paginationParameters = request.Adapt<PaginationParameters>();

        var query = request.Adapt<GetAllProducts>();

        query = query with { PaginationParameters = paginationParameters };

        var result = await _mediator.Send(query);

        var categories = await _mediator.Send(new GetAllCategories());

        var viewModel = new ProductsViewModel
        {
            Products = result,
            Categories = categories.Results
        };

        return View(viewModel);
    }
    
    [HttpGet("products/AddProduct")]
    public async Task<IActionResult> AddProduct()
    {
        var categoriesQuery = new GetAllCategories();

        var categories = await _mediator.Send(categoriesQuery);

        var viewModel = new AddProductViewModel
        {
            Categories = categories.Results
        };

        return View(viewModel);
    }

    [HttpPost("products/AddProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductRequest request)
    {
        var categories = await _mediator.Send(new GetAllCategories());

        var viewModel = new AddProductViewModel
        {
            Categories = categories.Results
        };

        var command = request.Adapt<CreateProductV2>();

        var result = await _mediator.Send(command);

        return result.Match(dto =>
        {
            ViewData[Constants.ViewDataKeys.Message] = Constants.Messages.CreateSuccessful;

            return View(viewModel);
        }, failure =>
        {
            if (failure.State == State.ValidationFailure)
            {
                ModelState.SetModelStateErrors((failure.GetResponseData() as ValidationFailureResponse)!);

                return View(viewModel);
            }

            return failure.ToResponse();
        });
    }

    [HttpGet("products/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> ProductDetails([FromRoute] string slug)
    {
        var getProductQuery = new GetProductBySlug(slug);

        var productQueryResult = await _mediator.Send(getProductQuery);

        return productQueryResult.Match(dto =>
        {
            var viewModel = new ProductDetailsViewModel
            {
                Product = dto
            };

            return View(viewModel);
        }, failure => failure.ToResponse());
    }

    [HttpPost("products/{slug}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct([FromRoute] string slug, [FromForm] EditProductRequest request)
    {
        var productQueryResult = await _mediator.Send(new GetProductBySlug(slug));

        if (productQueryResult.IsLeft)
        {
            return (productQueryResult.Case as FailureResponse)!.ToResponse();
        }

        var product = (productQueryResult.Case as ProductDto)!;

        var command = request.Adapt<UpdateProductV2>();

        command = command with { Id = product.Id };
    
        var result = await _mediator.Send(command);

        return result.Match(dto =>
        {
            var viewModel = new ProductDetailsViewModel
            {
                Product = dto
            };

            ViewData[Constants.ViewDataKeys.Message] = Constants.Messages.UpdateSuccessful;
            
            return View("ProductDetails", viewModel);
        }, failure =>
        {
            if (failure.State != State.ValidationFailure)
            {
                return failure.ToResponse();
            }

            ModelState.SetModelStateErrors((failure.GetResponseData() as ValidationFailureResponse)!);

            var viewModel = new ProductDetailsViewModel
            {
                Product = product
            };

            ViewData[Constants.ViewDataKeys.Message] = Constants.Messages.UpdateSuccessful;
            
            return View("ProductDetails", viewModel);
        });
    }

    [HttpDelete("products/{id:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        var command = new DeleteProduct(id);

        var result = await _mediator.Send(command);

        return result.Match(_ => NoContent(), failure => failure.ToResponse());
    }
}