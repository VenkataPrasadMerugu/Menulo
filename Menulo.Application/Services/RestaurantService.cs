using Menulo.Application.Abstractions.Persistence;
using Menulo.Application.Abstractions.Storage;
using Menulo.Application.Abstractions.Utilities;
using Menulo.Application.Common.Branding;
using Menulo.Application.Common.Pricing;
using Menulo.Application.Common.Results;
using Menulo.Application.DTOs.Menu;
using Menulo.Application.DTOs.Restaurants;
using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Application.Services;

public sealed class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ISlugService _slugService;
    private readonly IFileStorage _fileStorage;

    public RestaurantService(
        IRestaurantRepository restaurantRepository,
        IMenuItemRepository menuItemRepository,
        ISlugService slugService,
        IFileStorage fileStorage)
    {
        _restaurantRepository = restaurantRepository;
        _menuItemRepository = menuItemRepository;
        _slugService = slugService;
        _fileStorage = fileStorage;
    }

    public async Task<DashboardDto> GetDashboardAsync(int ownerUserId, string baseUrl, int? restaurantId = null, CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetByOwnerIdsAsync(ownerUserId, cancellationToken);
        var selectedRestaurant = ResolveSelectedRestaurant(restaurants, restaurantId);

        if (selectedRestaurant is null)
        {
            return new DashboardDto
            {
                Restaurants = []
            };
        }

        var items = await _menuItemRepository.GetByRestaurantIdAsync(ownerUserId, selectedRestaurant.Id, cancellationToken);
        var publicPath = BuildPublicMenuPath(selectedRestaurant.Slug);
        var palette = ResolvePalette(selectedRestaurant.PaletteKey);

        return new DashboardDto
        {
            RestaurantName = selectedRestaurant.Name,
            RestaurantSlug = selectedRestaurant.Slug,
            RestaurantId = selectedRestaurant.Id,
            BranchName = selectedRestaurant.BranchName,
            Address = selectedRestaurant.Address,
            LogoPath = selectedRestaurant.LogoPath,
            PrimaryColor = selectedRestaurant.PrimaryColor ?? palette.PrimaryColor,
            SecondaryColor = selectedRestaurant.SecondaryColor ?? palette.SecondaryColor,
            CurrencyCode = selectedRestaurant.CurrencyCode,
            CurrencySymbol = CurrencyCatalog.GetSymbol(selectedRestaurant.CurrencyCode),
            TotalMenuItems = items.Count,
            ActiveItemCount = items.Count(x => x.Status == Domain.Modules.Menu.MenuItemStatus.Active),
            OutOfStockItemCount = items.Count(x => x.Status == Domain.Modules.Menu.MenuItemStatus.OutOfStock),
            PublicMenuUrl = $"{baseUrl.TrimEnd('/')}{publicPath}",
            HasRestaurantProfile = true,
            PreviewItems = items
                .Where(x => x.Status != Domain.Modules.Menu.MenuItemStatus.Inactive)
                .OrderByDescending(x => x.Status == Domain.Modules.Menu.MenuItemStatus.Active)
                .ThenBy(x => x.Name)
                .Take(3)
                .Select(MapMenuItem)
                .ToList(),
            Restaurants = restaurants.Select(MapSummary).ToList()
        };
    }

    public async Task<RestaurantProfileDto?> GetOwnedRestaurantAsync(int ownerUserId, int? restaurantId = null, CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetByOwnerIdsAsync(ownerUserId, cancellationToken);
        var restaurant = ResolveSelectedRestaurant(restaurants, restaurantId);
        return restaurant is null ? null : MapProfile(restaurant);
    }

    public async Task<OperationResult> UpsertProfileAsync(int ownerUserId, RestaurantProfileRequest request, CancellationToken cancellationToken = default)
    {
        Restaurant? existingRestaurant = null;
        if (request.RestaurantId.HasValue)
        {
            existingRestaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, request.RestaurantId.Value, cancellationToken);
        }

        if (existingRestaurant is null)
        {
            var generatedSlug = await _slugService.GenerateUniqueSlugAsync(request.Name, null, null, cancellationToken);
            var logoPath = request.LogoUpload is null ? null : await _fileStorage.SaveAsync(request.LogoUpload, cancellationToken);
            var restaurant = Restaurant.Create(ownerUserId, request.Name, generatedSlug, request.CurrencyCode, request.BranchName, request.Address);
            restaurant.UpdateBranding(null, null, null, logoPath);
            await _restaurantRepository.AddAsync(restaurant, cancellationToken);
            await _restaurantRepository.SaveChangesAsync(cancellationToken);

            if (request.ImportSourceRestaurantId.HasValue)
            {
                var sourceRestaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, request.ImportSourceRestaurantId.Value, cancellationToken);
                if (sourceRestaurant is not null && sourceRestaurant.Id != restaurant.Id)
                {
                    var sourceItems = await _menuItemRepository.GetDirectByRestaurantIdAsync(sourceRestaurant.Id, cancellationToken);
                    foreach (var sourceItem in sourceItems)
                    {
                        var importedItem = Domain.Modules.Menu.MenuItem.Create(
                            restaurant.Id,
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
                }
            }

            return OperationResult.Success(restaurant.Id);
        }

        var generatedUpdateSlug = await _slugService.GenerateUniqueSlugAsync(request.Name, null, existingRestaurant.Id, cancellationToken);
        string? newLogoPath = null;
        if (request.LogoUpload is not null)
        {
            newLogoPath = await _fileStorage.SaveAsync(request.LogoUpload, cancellationToken);
            await _fileStorage.DeleteAsync(existingRestaurant.LogoPath, cancellationToken);
        }

        existingRestaurant.UpdateProfile(request.Name, generatedUpdateSlug, request.BranchName, request.Address, newLogoPath);
        existingRestaurant.UpdateCurrency(request.CurrencyCode);
        await _restaurantRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success(existingRestaurant.Id);
    }

    public async Task<OperationResult> UpdateBrandingAsync(int ownerUserId, int restaurantId, BrandingSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerUserId, restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return OperationResult.Failure("Create or select a restaurant before configuring branding.");
        }

        string? newLogoPath = null;
        if (request.LogoUpload is not null)
        {
            newLogoPath = await _fileStorage.SaveAsync(request.LogoUpload, cancellationToken);
            await _fileStorage.DeleteAsync(restaurant.LogoPath, cancellationToken);
        }

        var palette = ResolvePalette(request.PaletteKey);
        var primaryColor = string.IsNullOrWhiteSpace(request.PrimaryColor) ? palette.PrimaryColor : request.PrimaryColor;
        var secondaryColor = string.IsNullOrWhiteSpace(request.SecondaryColor) ? palette.SecondaryColor : request.SecondaryColor;

        restaurant.UpdateBranding(primaryColor, secondaryColor, request.PaletteKey, newLogoPath);
        await _restaurantRepository.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<PublicMenuDto?> GetPublicMenuAsync(string restaurantSlug, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetBySlugAsync(restaurantSlug, cancellationToken);
        if (restaurant is null)
        {
            return null;
        }

        var palette = ResolvePalette(restaurant.PaletteKey);
        var items = await _menuItemRepository.GetPublicItemsByRestaurantIdAsync(restaurant.OwnerUserId, restaurant.Id, cancellationToken);

        return new PublicMenuDto
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name,
            RestaurantSlug = restaurant.Slug,
            BranchName = restaurant.BranchName,
            Address = restaurant.Address,
            LogoPath = restaurant.LogoPath,
            PrimaryColor = restaurant.PrimaryColor ?? palette.PrimaryColor,
            SecondaryColor = restaurant.SecondaryColor ?? palette.SecondaryColor,
            CurrencyCode = restaurant.CurrencyCode,
            CurrencySymbol = CurrencyCatalog.GetSymbol(restaurant.CurrencyCode),
            Items = items.Select(MapMenuItem).ToList()
        };
    }

    public async Task<bool> IsSlugAvailableAsync(string slug, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default)
    {
        return !await _restaurantRepository.SlugExistsAsync(slug, ignoreRestaurantId, cancellationToken);
    }

    private static Restaurant? ResolveSelectedRestaurant(IReadOnlyList<Restaurant> restaurants, int? restaurantId)
    {
        return restaurantId.HasValue
            ? restaurants.FirstOrDefault(x => x.Id == restaurantId.Value) ?? restaurants.FirstOrDefault()
            : restaurants.FirstOrDefault();
    }

    private static RestaurantProfileDto MapProfile(Restaurant restaurant)
    {
        return new RestaurantProfileDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            OwnerUserId = restaurant.OwnerUserId,
            Slug = restaurant.Slug,
            BranchName = restaurant.BranchName,
            Address = restaurant.Address,
            LogoPath = restaurant.LogoPath,
            PrimaryColor = restaurant.PrimaryColor,
            SecondaryColor = restaurant.SecondaryColor,
            PaletteKey = restaurant.PaletteKey,
            CurrencyCode = restaurant.CurrencyCode,
            PublicMenuPath = BuildPublicMenuPath(restaurant.Slug)
        };
    }

    private static RestaurantSummaryDto MapSummary(Restaurant restaurant)
    {
        return new RestaurantSummaryDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Slug = restaurant.Slug,
            BranchName = restaurant.BranchName,
            PublicMenuPath = BuildPublicMenuPath(restaurant.Slug)
        };
    }

    private static MenuItemDto MapMenuItem(Domain.Modules.Menu.MenuItem item)
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

    private static BrandPalette ResolvePalette(string? paletteKey) => BrandPaletteCatalog.Resolve(paletteKey);

    private static string BuildPublicMenuPath(string restaurantSlug) => $"/{restaurantSlug}/menu";
}
