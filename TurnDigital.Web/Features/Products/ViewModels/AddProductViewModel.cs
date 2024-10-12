using System.ComponentModel.DataAnnotations;
using TurnDigital.Domain.Features.Categories.Dtos;

namespace TurnDigital.Web.Features.Products.ViewModels;

public class AddProductViewModel
{
    public required IReadOnlyList<CategoryDto> Categories { get; init; } 
    
    private string _name = string.Empty;
    
    [Required]
    public string Name
    {
        get => _name;
        set => _name = !string.IsNullOrEmpty(value) ? value.Trim() : value;
    }
    
    [Required]
    public IFormFile? Image { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}