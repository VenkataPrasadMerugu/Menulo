using Menulo.Domain.Common;

namespace Menulo.Domain.Modules.Menu;

public sealed class MenuItem : AuditableEntity
{
    private readonly List<MenuItemImage> _images = [];

    private MenuItem()
    {
    }

    public int RestaurantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Serves { get; private set; } = 1;
    public MenuCategory Category { get; private set; } = MenuCategory.MainCourse;
    public FoodType FoodType { get; private set; } = FoodType.Veg;
    public MenuItemStatus Status { get; private set; } = MenuItemStatus.Active;
    public IReadOnlyCollection<MenuItemImage> Images => _images;

    public static MenuItem Create(int restaurantId, string name, decimal price, int serves, MenuCategory category, FoodType foodType)
    {
        return new MenuItem
        {
            RestaurantId = restaurantId,
            Name = name.Trim(),
            Price = price,
            Serves = serves,
            Category = category,
            FoodType = foodType,
            Status = MenuItemStatus.Active
        };
    }

    public void Update(string name, decimal price, int serves, MenuCategory category, FoodType foodType)
    {
        Name = name.Trim();
        Price = price;
        Serves = serves;
        Category = category;
        FoodType = foodType;

        Touch();
    }

    public void SetStatus(MenuItemStatus status)
    {
        Status = status;
        Touch();
    }

    public void ReplaceImages(IEnumerable<string> imagePaths)
    {
        _images.Clear();
        foreach (var image in imagePaths.Where(x => !string.IsNullOrWhiteSpace(x)).Select((path, index) => new { path, index }))
        {
            _images.Add(MenuItemImage.Create(Id, image.path, image.index + 1));
        }

        Touch();
    }
}
