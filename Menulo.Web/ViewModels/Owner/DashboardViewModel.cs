namespace Menulo.Web.ViewModels.Owner;

public sealed class DashboardViewModel
{
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantSlug { get; set; }
    public int? RestaurantId { get; set; }
    public string? BranchName { get; set; }
    public string? Address { get; set; }
    public string? PublicMenuUrl { get; set; }
    public string? QrDownloadUrl { get; set; }
    public string? LogoPath { get; set; }
    public string PrimaryColor { get; set; } = "#A63A50";
    public string SecondaryColor { get; set; } = "#F4B860";
    public string CurrencySymbol { get; set; } = "Rs";
    public int TotalMenuItems { get; set; }
    public int ActiveItemCount { get; set; }
    public int OutOfStockItemCount { get; set; }
    public bool HasRestaurantProfile { get; set; }
    public IReadOnlyList<DashboardPreviewItemViewModel> PreviewItems { get; set; } = [];
    public IReadOnlyList<RestaurantSwitcherViewModel> Restaurants { get; set; } = [];
}
