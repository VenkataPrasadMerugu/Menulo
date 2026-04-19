namespace Menulo.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<OwnerIdentityDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
}
