using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Application.Abstractions.Persistence;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByOwnerIdAsync(int ownerUserId, int? restaurantId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Restaurant>> GetByOwnerIdsAsync(int ownerUserId, CancellationToken cancellationToken = default);
    Task<Restaurant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Restaurant?> GetByIdAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
