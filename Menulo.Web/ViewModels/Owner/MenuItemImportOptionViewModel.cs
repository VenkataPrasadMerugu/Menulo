namespace Menulo.Web.ViewModels.Owner;

public sealed class MenuItemImportOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? PrimaryImagePath { get; set; }
}
