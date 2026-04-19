namespace Menulo.Application.DTOs.Restaurants;

public sealed class RestaurantSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? BranchName { get; init; }
    public string PublicMenuPath { get; init; } = string.Empty;
}
