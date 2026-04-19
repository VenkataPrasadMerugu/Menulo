using Menulo.Application.DTOs.Menu;

namespace Menulo.Web.ViewModels.Public;

public sealed class PublicMenuViewModel
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantSlug { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string? Address { get; set; }
    public string? LogoPath { get; set; }
    public string PrimaryColor { get; set; } = "#A63A50";
    public string SecondaryColor { get; set; } = "#F4B860";
    public string CurrencySymbol { get; set; } = "Rs";
    public IReadOnlyList<MenuItemDto> Items { get; set; } = [];
}
