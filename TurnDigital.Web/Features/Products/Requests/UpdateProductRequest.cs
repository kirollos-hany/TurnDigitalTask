namespace TurnDigital.Web.Features.Products.Requests;

public class UpdateProductRequest
{
    private string? _name = null;
    
    public string? Name
    {
        get => _name;
        set => _name = !string.IsNullOrEmpty(value) ? value.Trim() : value;
    }
    
    public IFormFile? Image { get; set; }
    
    public double? Price { get; set; }
    
    public string? Description { get; set; }
    
    public int? CategoryId { get; set; }
}