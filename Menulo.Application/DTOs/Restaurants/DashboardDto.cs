using Menulo.Domain.Modules.Restaurants;
using Menulo.Application.DTOs.Menu;

namespace Menulo.Application.DTOs.Restaurants;

public sealed class DashboardDto
{
    public string RestaurantName { get; init; } = "Set up your restaurant";
    public string? RestaurantSlug { get; init; }
    public int? RestaurantId { get; init; }
    public string? BranchName { get; init; }
    public string? Address { get; init; }
    public string? LogoPath { get; init; }
    public string PrimaryColor { get; init; } = "#A63A50";
    public string SecondaryColor { get; init; } = "#F4B860";
    public string CurrencySymbol { get; init; } = "Rs";
    public CurrencyCode CurrencyCode { get; init; } = CurrencyCode.INR;
    public int TotalMenuItems { get; init; }
    public int ActiveItemCount { get; init; }
    public int OutOfStockItemCount { get; init; }
    public string? PublicMenuUrl { get; init; }
    public bool HasRestaurantProfile { get; init; }
    public IReadOnlyList<MenuItemDto> PreviewItems { get; init; } = [];
    public IReadOnlyList<RestaurantSummaryDto> Restaurants { get; init; } = [];
}
