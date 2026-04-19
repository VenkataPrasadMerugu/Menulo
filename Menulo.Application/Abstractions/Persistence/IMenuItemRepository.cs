using Menulo.Domain.Modules.Menu;

namespace Menulo.Application.Abstractions.Persistence;

public interface IMenuItemRepository
{
    Task<IReadOnlyList<MenuItem>> GetDirectByRestaurantIdAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetByRestaurantIdAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetPublicItemsByRestaurantIdAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    void Remove(MenuItem menuItem);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
