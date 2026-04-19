using Menulo.Application.Abstractions.Persistence;
using Menulo.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Menulo.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly MenuloDbContext _dbContext;

    public UserRepository(MenuloDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OwnerIdentityDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => new OwnerIdentityDto
            {
                UserId = x.Id,
                FullName = x.FullName,
                Email = x.Email ?? x.UserName ?? string.Empty
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
