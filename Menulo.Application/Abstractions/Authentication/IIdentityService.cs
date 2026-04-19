using Menulo.Application.DTOs.Accounts;

namespace Menulo.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<AuthResult> RegisterOwnerAsync(RegisterOwnerRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> PasswordSignInAsync(LoginOwnerRequest request, CancellationToken cancellationToken = default);
    Task SignOutAsync();
}
