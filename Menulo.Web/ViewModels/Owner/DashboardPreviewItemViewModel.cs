namespace Menulo.Web.ViewModels.Owner;

public sealed class DashboardPreviewItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string CategoryLabel { get; set; } = string.Empty;
    public string FoodTypeLabel { get; set; } = string.Empty;
    public int Serves { get; set; }
    public string Price { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public bool IsOutOfStock { get; set; }
}
