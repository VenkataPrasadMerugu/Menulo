using Menulo.Application.Abstractions.Persistence;
using Menulo.Domain.Modules.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Menulo.Infrastructure.Persistence.Repositories;

internal sealed class RestaurantRepository : IRestaurantRepository
{
    private readonly MenuloDbContext _dbContext;

    public RestaurantRepository(MenuloDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Restaurant?> GetByOwnerIdAsync(int ownerUserId, int? restaurantId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Restaurants.Where(x => x.OwnerUserId == ownerUserId);
        if (restaurantId.HasValue)
        {
            query = query.Where(x => x.Id == restaurantId.Value);
        }

        return query.OrderBy(x => x.Name).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Restaurant>> GetByOwnerIdsAsync(int ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Restaurants
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Restaurant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _dbContext.Restaurants.SingleOrDefaultAsync(x => x.Slug == slug.ToLower(), cancellationToken);
    }

    public Task<Restaurant?> GetByIdAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Restaurants.SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
    }

    public Task<bool> SlugExistsAsync(string slug, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.Restaurants.AnyAsync(
            x => x.Slug == slug.ToLower() && (!ignoreRestaurantId.HasValue || x.Id != ignoreRestaurantId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await _dbContext.Restaurants.AddAsync(restaurant, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
