using Menulo.Domain.Modules.Restaurants;
using Menulo.Application.DTOs.Menu;

namespace Menulo.Application.DTOs.Restaurants;

public sealed class PublicMenuDto
{
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public string RestaurantSlug { get; init; } = string.Empty;
    public string? BranchName { get; init; }
    public string? Address { get; init; }
    public string? LogoPath { get; init; }
    public string PrimaryColor { get; init; } = "#A63A50";
    public string SecondaryColor { get; init; } = "#F4B860";
    public string CurrencySymbol { get; init; } = "Rs";
    public CurrencyCode CurrencyCode { get; init; } = CurrencyCode.INR;
    public IReadOnlyList<MenuItemDto> Items { get; init; } = [];
}
