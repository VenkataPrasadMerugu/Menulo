using Menulo.Application.Abstractions.Authentication;
using Menulo.Application.Abstractions.CurrentUser;
using Menulo.Application.DTOs.Accounts;
using Menulo.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menulo.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public AccountController(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    [AllowAnonymous]
    [HttpGet("/account/register")]
    public IActionResult Register()
    {
        if (_currentUserService.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Owner" });
        }

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost("/account/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = await _identityService.RegisterOwnerAsync(new RegisterOwnerRequest
        {
            FullName = viewModel.FullName,
            Email = viewModel.Email,
            Password = viewModel.Password,
            ConfirmPassword = viewModel.ConfirmPassword
        });

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return View(viewModel);
        }

        TempData["Success"] = "Owner account created.";
        return RedirectToAction("Index", "Dashboard", new { area = "Owner" });
    }

    [AllowAnonymous]
    [HttpGet("/account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_currentUserService.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Owner" });
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = await _identityService.PasswordSignInAsync(new LoginOwnerRequest
        {
            Email = viewModel.Email,
            Password = viewModel.Password,
            RememberMe = viewModel.RememberMe
        });

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Login failed.");
            return View(viewModel);
        }

        TempData["Success"] = "Logged in successfully.";
        return LocalRedirect(string.IsNullOrWhiteSpace(viewModel.ReturnUrl) ? "/owner/dashboard" : viewModel.ReturnUrl);
    }

    [Authorize]
    [HttpPost("/account/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _identityService.SignOutAsync();
        TempData["Success"] = "Signed out.";
        return RedirectToAction(nameof(Login));
    }
}
