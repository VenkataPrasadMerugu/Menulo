using Menulo.Application.DTOs.Menu;

namespace Menulo.Web.ViewModels.Owner;

public sealed class MenuItemListViewModel
{
    public int RestaurantId { get; set; }
    public IReadOnlyList<RestaurantSwitcherViewModel> Restaurants { get; set; } = [];
    public IReadOnlyList<RestaurantSwitcherViewModel> ImportSources { get; set; } = [];
    public int? ImportSourceRestaurantId { get; set; }
    public IReadOnlyList<MenuItemImportOptionViewModel> ImportableItems { get; set; } = [];
    public IReadOnlyList<MenuItemDto> Items { get; set; } = [];
}
