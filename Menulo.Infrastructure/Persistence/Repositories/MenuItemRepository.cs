using Menulo.Application.Abstractions.Persistence;
using Menulo.Domain.Modules.Menu;
using Microsoft.EntityFrameworkCore;

namespace Menulo.Infrastructure.Persistence.Repositories;

internal sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly MenuloDbContext _dbContext;

    public MenuItemRepository(MenuloDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MenuItem>> GetByRestaurantIdAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MenuItems
            .Where(x => x.RestaurantId == restaurantId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetDirectByRestaurantIdAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MenuItems
            .Where(x => x.RestaurantId == restaurantId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetPublicItemsByRestaurantIdAsync(int ownerUserId, int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MenuItems
            .Where(x =>
                x.RestaurantId == restaurantId
                && (x.Status == MenuItemStatus.Active || x.Status == MenuItemStatus.OutOfStock))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.MenuItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        await _dbContext.MenuItems.AddAsync(menuItem, cancellationToken);
    }

    public void Remove(MenuItem menuItem)
    {
        _dbContext.MenuItems.Remove(menuItem);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
