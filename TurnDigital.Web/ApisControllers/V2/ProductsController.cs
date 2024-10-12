using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TurnDigital.Application.Features.Products.Commands.V2;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Security.Enums;
using TurnDigital.Domain.Web.Interfaces;
using TurnDigital.Web.Features.Products.Requests;
using TurnDigital.Web.Mappers;

namespace TurnDigital.Web.ApisControllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("2.0")]
[Authorize(Roles = nameof(Roles.TurnDigitalAdmin))]
public class ProductsV2Controller : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly IBaseUrlProvider _baseUrlProvider;

    public ProductsV2Controller(IMediator mediator, IBaseUrlProvider baseUrlProvider)
    {
        _mediator = mediator;
        _baseUrlProvider = baseUrlProvider;
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDto))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationFailureResponse))]
    [SwaggerOperation(
        Summary = "Admin create product",
        Description = "Admin create a new product",
        OperationId = "Products.Create",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
    {
        var command = request.Adapt<CreateProductV2>();

        var result = await _mediator.Send(command);

        return result.Match(dto => Created($"{_baseUrlProvider.GetBaseUrl()}/api/v1/products/{dto.Id}", dto),
            failure => failure.ToResponse());
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationFailureResponse))]
    [SwaggerOperation(
        Summary = "Admin partial update product",
        Description = "Admin partial update product by id",
        OperationId = "Products.Update",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductRequest request)
    {
        var command = request.Adapt<UpdateProductV2>();

        command = command with { Id = id };

        var result = await _mediator.Send(command);

        return result.Match(Ok, failure => failure.ToResponse());
    }
}