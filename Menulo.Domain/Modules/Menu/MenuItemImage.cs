using Menulo.Domain.Common;

namespace Menulo.Domain.Modules.Menu;

public sealed class MenuItemImage : AuditableEntity
{
    private MenuItemImage()
    {
    }

    public int MenuItemId { get; private set; }
    public string ImagePath { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    public static MenuItemImage Create(int menuItemId, string imagePath, int sortOrder)
    {
        return new MenuItemImage
        {
            MenuItemId = menuItemId,
            ImagePath = imagePath,
            SortOrder = sortOrder
        };
    }
}
