using System.ComponentModel.DataAnnotations;
using Menulo.Domain.Modules.Menu;

namespace Menulo.Web.ViewModels.Owner;

public sealed class MenuItemFormViewModel
{
    public int? Id { get; set; }
    public int RestaurantId { get; set; }

    [Required]
    [Display(Name = "Item name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0.01", "999999")]
    public decimal Price { get; set; }

    [Display(Name = "Serves")]
    [Range(1, 50)]
    public int Serves { get; set; } = 1;

    [Display(Name = "Category")]
    public MenuCategory Category { get; set; } = MenuCategory.MainCourse;

    [Display(Name = "Food type")]
    public FoodType FoodType { get; set; } = FoodType.Veg;

    [Display(Name = "Item images")]
    public List<IFormFile> Images { get; set; } = [];

    public IReadOnlyList<string> ExistingImagePaths { get; set; } = [];
    public IReadOnlyList<RestaurantSwitcherViewModel> Restaurants { get; set; } = [];
}
