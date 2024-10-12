using System.ComponentModel.DataAnnotations;

namespace TurnDigital.Web.Features.Categories.Requests;

public class EditCategoryRequest
{
    private string _name = string.Empty;
    
    [Required]
    public string Name
    {
        get => _name;
        set => _name = !string.IsNullOrEmpty(value) ? value.Trim() : value;
    }
}