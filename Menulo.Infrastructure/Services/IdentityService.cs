using Menulo.Application.Abstractions.Authentication;
using Menulo.Application.DTOs.Accounts;
using Menulo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Menulo.Infrastructure.Services;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResult> RegisterOwnerAsync(RegisterOwnerRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new AuthResult(false, string.Join(" ", result.Errors.Select(x => x.Description)));
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return new AuthResult(true);
    }

    public async Task<AuthResult> PasswordSignInAsync(LoginOwnerRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, "Invalid email or password.");
    }

    public Task SignOutAsync()
    {
        return _signInManager.SignOutAsync();
    }
}
