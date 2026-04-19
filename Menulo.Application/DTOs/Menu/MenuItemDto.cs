using Menulo.Domain.Modules.Menu;

namespace Menulo.Application.DTOs.Menu;

public sealed class MenuItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Serves { get; init; }
    public MenuCategory Category { get; init; }
    public FoodType FoodType { get; init; }
    public string? PrimaryImagePath { get; init; }
    public IReadOnlyList<string> ImagePaths { get; init; } = [];
    public MenuItemStatus Status { get; init; }
}
