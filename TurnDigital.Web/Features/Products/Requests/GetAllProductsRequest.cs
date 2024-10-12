using Microsoft.AspNetCore.Mvc;
using TurnDigital.Web.Common.Requests;

namespace TurnDigital.Web.Features.Products.Requests;

public class GetAllProductsRequest : PaginationRequest
{
    [FromQuery] public int? CategoryId { get; set; }
    
    [FromQuery] public string? Name { get; set; }
}