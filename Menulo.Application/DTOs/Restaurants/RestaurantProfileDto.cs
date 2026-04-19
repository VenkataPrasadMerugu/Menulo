using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Application.DTOs.Restaurants;

public sealed class RestaurantProfileDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int OwnerUserId { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? BranchName { get; init; }
    public string? Address { get; init; }
    public string? LogoPath { get; init; }
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string? PaletteKey { get; init; }
    public CurrencyCode CurrencyCode { get; init; }
    public string PublicMenuPath { get; init; } = string.Empty;
}
