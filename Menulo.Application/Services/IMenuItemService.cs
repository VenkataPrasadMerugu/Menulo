using Menulo.Application.Common.Results;
using Menulo.Application.DTOs.Menu;
using Menulo.Domain.Modules.Menu;

namespace Menulo.Application.Services;

public interface IMenuItemService
{
    Task<IReadOnlyList<MenuItemDto>> GetOwnedItemsAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default);
    Task<MenuItemDto?> GetOwnedItemAsync(int ownerUserId, int restaurantId, int menuItemId, CancellationToken cancellationToken = default);
    Task<OperationResult> CreateAsync(int ownerUserId, int restaurantId, MenuItemUpsertRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult> ImportAsync(int ownerUserId, int targetRestaurantId, int sourceRestaurantId, IReadOnlyCollection<int> sourceMenuItemIds, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateAsync(int ownerUserId, int restaurantId, int menuItemId, MenuItemUpsertRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(int ownerUserId, int restaurantId, int menuItemId, CancellationToken cancellationToken = default);
    Task<OperationResult> SetStatusAsync(int ownerUserId, int restaurantId, int menuItemId, MenuItemStatus status, CancellationToken cancellationToken = default);
}
