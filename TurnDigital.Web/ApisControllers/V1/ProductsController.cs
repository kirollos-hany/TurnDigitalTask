using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TurnDigital.Application.Features.Products.Commands;
using TurnDigital.Application.Features.Products.Commands.V1;
using TurnDigital.Application.Features.Products.Queries;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Query.Pagination;
using TurnDigital.Domain.Security.Enums;
using TurnDigital.Domain.Web.Interfaces;
using TurnDigital.Web.Features.Products.Requests;
using TurnDigital.Web.Mappers;

namespace TurnDigital.Web.ApisControllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/products")]
[Route("api/products")]
[ApiVersion("1.0")]
[Authorize(Roles = nameof(Roles.TurnDigitalAdmin))]
[ApiExplorerSettings(GroupName = "v1")]
public class ProductsV1Controller : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly IBaseUrlProvider _baseUrlProvider;

    public ProductsV1Controller(IMediator mediator, IBaseUrlProvider baseUrlProvider)
    {
        _mediator = mediator;
        _baseUrlProvider = baseUrlProvider;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<ProductDto>))]
    [SwaggerOperation(
        Summary = "User get list of products",
        Description = "User get list of products, can be filtered by category and/or product name",
        OperationId = "Products.GetAllProducts",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> Get([AsParameters] GetAllProductsRequest request)
    {
        var paginationParameters = request.Adapt<PaginationParameters>();

        var query = request.Adapt<GetAllProducts>();

        query = query with { PaginationParameters = paginationParameters };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
    [SwaggerOperation(
        Summary = "User get product details by id",
        Description = "User get product details by id",
        OperationId = "Products.GetProductById",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new GetProductById(id);

        var result = await _mediator.Send(query);

        return result.Match(Ok, failure => failure.ToResponse());
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
        var command = request.Adapt<CreateProductV1>();

        var result = await _mediator.Send(command);

        return result.Match(dto => Created($"{_baseUrlProvider.GetBaseUrl()}/api/v1/products/{dto.Id}", dto),
            failure => failure.ToResponse());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationFailureResponse))]
    [SwaggerOperation(
        Summary = "Admin update product",
        Description = "Admin update product by id",
        OperationId = "Products.Update",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] EditProductRequest request)
    {
        var command = request.Adapt<UpdateProductV1>();

        command = command with { Id = id };

        var result = await _mediator.Send(command);

        return result.Match(Ok, failure => failure.ToResponse());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(UnauthorizedResponse))]
    [SwaggerOperation(
        Summary = "Admin delete product",
        Description = "Admin delete product by id",
        OperationId = "Products.Delete",
        Tags = new[] { "Products Endpoints" })]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var command = new DeleteProduct(id);

        var result = await _mediator.Send(command);

        return result.Match(_ => NoContent(), failure => failure.ToResponse());
    }
}