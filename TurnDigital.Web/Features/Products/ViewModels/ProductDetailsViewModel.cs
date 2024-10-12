using System.ComponentModel.DataAnnotations;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Products.Dtos;

namespace TurnDigital.Web.Features.Products.ViewModels;

public class ProductDetailsViewModel
{
    public required ProductDto Product { get; init; }

    private string _name = string.Empty;
    
    [Required]
    public string Name
    {
        get => _name;
        set => _name = !string.IsNullOrEmpty(value) ? value.Trim() : value;
    }
    
    public IFormFile? Image { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}