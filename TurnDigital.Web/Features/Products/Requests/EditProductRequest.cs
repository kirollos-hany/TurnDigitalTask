using System.ComponentModel.DataAnnotations;

namespace TurnDigital.Web.Features.Products.Requests;

public class EditProductRequest
{
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
}