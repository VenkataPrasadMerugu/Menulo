using Menulo.Application.Abstractions.Persistence;
using Menulo.Application.Abstractions.Storage;
using Menulo.Application.Common.Results;
using Menulo.Application.DTOs.Menu;
using Menulo.Domain.Modules.Menu;

namespace Menulo.Application.Services;

public sealed class MenuItemService : IMenuItemService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IFileStorage _fileStorage;

    public MenuItemService(
        IRestaurantRepository restaurantRepository,
        IMenuItemRepository menuItemRepository,
        IFileStorage fileStorage)
    {
        _restaurantRepository = restaurantRepository;
        _menuItemRepository = menuItemRepository;
        _fileStorage = fileStorage;
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetOwnedItemsAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return [];
        }

        var items = await _menuItemRepository.GetByRestaurantIdAsync(ownerUserId, restaurant.Id, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<MenuItemDto?> GetOwnedItemAsync(int ownerUserId, int restaurantId, int menuItemId, CancellationToken cancellationToken = default)
    {
        var item = await GetOwnedEntityAsync(ownerUserId, restaurantId, menuItemId, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<OperationResult> CreateAsync(int ownerUserId, int restaurantId, MenuItemUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return OperationResult.Failure("Select a restaurant before adding menu items.");
        }

        var menuItem = MenuItem.Create(restaurant.Id, request.Name, request.Price, request.Serves, request.Category, request.FoodType);
        await _menuItemRepository.AddAsync(menuItem, cancellationToken);
        await _menuItemRepository.SaveChangesAsync(cancellationToken);

        var imagePaths = await SaveImagesAsync(request.ImageUploads, cancellationToken);
        menuItem.ReplaceImages(imagePaths);
        await _menuItemRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> ImportAsync(int ownerUserId, int targetRestaurantId, int sourceRestaurantId, IReadOnlyCollection<int> sourceMenuItemIds, CancellationToken cancellationToken = default)
    {
        if (targetRestaurantId == sourceRestaurantId)
        {
            return OperationResult.Failure("Select a different source restaurant to import from.");
        }

        if (sourceMenuItemIds.Count == 0)
        {
            return OperationResult.Failure("Select at least one menu item to import.");
        }

        var targetRestaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, targetRestaurantId, cancellationToken);
        var sourceRestaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, sourceRestaurantId, cancellationToken);
        if (targetRestaurant is null || sourceRestaurant is null)
        {
            return OperationResult.Failure("Source or target restaurant not found.");
        }

        var sourceItems = await _menuItemRepository.GetDirectByRestaurantIdAsync(sourceRestaurantId, cancellationToken);
        sourceItems = sourceItems.Where(x => sourceMenuItemIds.Contains(x.Id)).ToList();
        if (sourceItems.Count == 0)
        {
            return OperationResult.Failure("The selected menu items were not found in the source restaurant.");
        }

        foreach (var sourceItem in sourceItems)
        {
            var importedItem = MenuItem.Create(
                targetRestaurant.Id,
                sourceItem.Name,
                sourceItem.Price,
                sourceItem.Serves,
                sourceItem.Category,
                sourceItem.FoodType);

            importedItem.SetStatus(sourceItem.Status);
            await _menuItemRepository.AddAsync(importedItem, cancellationToken);
            await _menuItemRepository.SaveChangesAsync(cancellationToken);

            var copiedImages = new List<string>();
            foreach (var image in sourceItem.Images.OrderBy(x => x.SortOrder))
            {
                var copiedPath = await _fileStorage.CopyAsync(image.ImagePath, "uploads/menu-items", cancellationToken);
                if (!string.IsNullOrWhiteSpace(copiedPath))
                {
                    copiedImages.Add(copiedPath);
                }
            }

            importedItem.ReplaceImages(copiedImages);
            await _menuItemRepository.SaveChangesAsync(cancellationToken);
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAsync(int ownerUserId, int restaurantId, int menuItemId, MenuItemUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var menuItem = await GetOwnedEntityAsync(ownerUserId, restaurantId, menuItemId, cancellationToken);
        if (menuItem is null)
        {
            return OperationResult.Failure("Menu item not found.");
        }

        menuItem.Update(request.Name, request.Price, request.Serves, request.Category, request.FoodType);

        if (request.ImageUploads.Count > 0)
        {
            foreach (var existingImage in menuItem.Images)
            {
                await _fileStorage.DeleteAsync(existingImage.ImagePath, cancellationToken);
            }

            var imagePaths = await SaveImagesAsync(request.ImageUploads, cancellationToken);
            menuItem.ReplaceImages(imagePaths);
        }

        await _menuItemRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int ownerUserId, int restaurantId, int menuItemId, CancellationToken cancellationToken = default)
    {
        var menuItem = await GetOwnedEntityAsync(ownerUserId, restaurantId, menuItemId, cancellationToken);
        if (menuItem is null)
        {
            return OperationResult.Failure("Menu item not found.");
        }

        foreach (var existingImage in menuItem.Images)
        {
            await _fileStorage.DeleteAsync(existingImage.ImagePath, cancellationToken);
        }

        _menuItemRepository.Remove(menuItem);
        await _menuItemRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> SetStatusAsync(int ownerUserId, int restaurantId, int menuItemId, MenuItemStatus status, CancellationToken cancellationToken = default)
    {
        var menuItem = await GetOwnedEntityAsync(ownerUserId, restaurantId, menuItemId, cancellationToken);
        if (menuItem is null)
        {
            return OperationResult.Failure("Menu item not found.");
        }

        menuItem.SetStatus(status);
        await _menuItemRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }

    private async Task<MenuItem?> GetOwnedEntityAsync(int ownerUserId, int restaurantId, int menuItemId, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return null;
        }

        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId, cancellationToken);
        return menuItem?.RestaurantId == restaurant.Id ? menuItem : null;
    }

    private async Task<List<string>> SaveImagesAsync(IReadOnlyList<FileUploadRequest> imageUploads, CancellationToken cancellationToken)
    {
        var paths = new List<string>();
        foreach (var upload in imageUploads)
        {
            paths.Add(await _fileStorage.SaveAsync(upload, cancellationToken));
        }

        return paths;
    }

    private static MenuItemDto Map(MenuItem item)
    {
        var orderedImages = item.Images.OrderBy(x => x.SortOrder).Select(x => x.ImagePath).ToList();
        return new MenuItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Serves = item.Serves,
            Category = item.Category,
            FoodType = item.FoodType,
            PrimaryImagePath = orderedImages.FirstOrDefault(),
            ImagePaths = orderedImages,
            Status = item.Status
        };
    }
}
