using Menulo.Application.Abstractions.CurrentUser;
using Menulo.Application.Abstractions.Storage;
using Menulo.Application.Common.Branding;
using Menulo.Application.DTOs.Restaurants;
using Menulo.Application.Services;
using Menulo.Web.Infrastructure;
using Menulo.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menulo.Web.Areas.Owner.Controllers;

[Area("Owner")]
[Authorize]
public sealed class RestaurantController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(ICurrentUserService currentUserService, IRestaurantService restaurantService)
    {
        _currentUserService = currentUserService;
        _restaurantService = restaurantService;
    }

    [HttpGet("/owner/account")]
    public async Task<IActionResult> Profile(int? restaurantId, bool createNew = false, CancellationToken cancellationToken = default)
    {
        if (!createNew)
        {
            restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        }

        var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        var profile = createNew
            ? null
            : await _restaurantService.GetOwnedRestaurantAsync(_currentUserService.UserId!.Value, restaurantId, cancellationToken);
        if (profile?.Id is int selectedRestaurantId)
        {
            HttpContext.Session.SetSelectedRestaurantId(selectedRestaurantId);
        }
        var viewModel = new RestaurantProfileViewModel
        {
            RestaurantId = profile?.Id,
            Restaurants = MapRestaurants(dashboard.Restaurants),
            ImportSources = MapRestaurants(dashboard.Restaurants),
            IsCreateMode = createNew || profile is null
        };

        if (profile is not null)
        {
            viewModel.Name = profile.Name;
            viewModel.BranchName = profile.BranchName;
            viewModel.Address = profile.Address;
            viewModel.ExistingLogoPath = profile.LogoPath;
            viewModel.PublicMenuPath = profile.PublicMenuPath;
            viewModel.CurrencyCode = profile.CurrencyCode;
        }

        return View(viewModel);
    }

    [HttpPost("/owner/account")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(RestaurantProfileViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            viewModel.ImportSources = MapRestaurants(dashboard.Restaurants.Where(x => x.Id != viewModel.RestaurantId).ToList());
            viewModel.IsCreateMode = !viewModel.RestaurantId.HasValue;
            return View(viewModel);
        }

        var request = new RestaurantProfileRequest
        {
            RestaurantId = viewModel.RestaurantId,
            Name = viewModel.Name,
            BranchName = viewModel.BranchName,
            Address = viewModel.Address,
            CurrencyCode = viewModel.CurrencyCode,
            ImportSourceRestaurantId = viewModel.ImportSourceRestaurantId,
            LogoUpload = await ToUploadAsync(viewModel.Logo, "uploads/logos", cancellationToken)
        };

        var result = await _restaurantService.UpsertProfileAsync(_currentUserService.UserId!.Value, request, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            viewModel.ImportSources = MapRestaurants(dashboard.Restaurants.Where(x => x.Id != viewModel.RestaurantId).ToList());
            viewModel.IsCreateMode = !viewModel.RestaurantId.HasValue;
            return View(viewModel);
        }

        if (result.EntityId.HasValue)
        {
            HttpContext.Session.SetSelectedRestaurantId(result.EntityId.Value);
        }

        TempData["Success"] = "Restaurant profile saved.";
        return RedirectToAction(nameof(Profile), new { restaurantId = result.EntityId ?? viewModel.RestaurantId ?? HttpContext.Session.GetSelectedRestaurantId() });
    }

    [HttpGet("/owner/branding")]
    public async Task<IActionResult> Branding(int? restaurantId, CancellationToken cancellationToken)
    {
        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        var profile = await _restaurantService.GetOwnedRestaurantAsync(_currentUserService.UserId!.Value, restaurantId, cancellationToken);
        if (profile?.Id is int selectedRestaurantId)
        {
            HttpContext.Session.SetSelectedRestaurantId(selectedRestaurantId);
        }
        var viewModel = new BrandingSettingsViewModel
        {
            PaletteOptions = GetPalettes(),
            Restaurants = MapRestaurants(dashboard.Restaurants),
            RestaurantId = profile?.Id ?? restaurantId ?? 0
        };

        if (profile is not null)
        {
            viewModel.PrimaryColor = profile.PrimaryColor ?? viewModel.PrimaryColor;
            viewModel.SecondaryColor = profile.SecondaryColor ?? viewModel.SecondaryColor;
            viewModel.PrimaryColorText = viewModel.PrimaryColor;
            viewModel.SecondaryColorText = viewModel.SecondaryColor;
            viewModel.ExistingLogoPath = profile.LogoPath;
            viewModel.RestaurantSlug = profile.PublicMenuPath;
            viewModel.PaletteKey = profile.PaletteKey ?? viewModel.PaletteKey;
        }

        return View(viewModel);
    }

    [HttpPost("/owner/branding")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Branding(BrandingSettingsViewModel viewModel, CancellationToken cancellationToken)
    {
        viewModel.PrimaryColor = viewModel.PrimaryColorText;
        viewModel.SecondaryColor = viewModel.SecondaryColorText;

        if (!ModelState.IsValid)
        {
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.PaletteOptions = GetPalettes();
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        var request = new BrandingSettingsRequest
        {
            PaletteKey = viewModel.PaletteKey,
            PrimaryColor = viewModel.PrimaryColor,
            SecondaryColor = viewModel.SecondaryColor,
            LogoUpload = await ToUploadAsync(viewModel.Logo, "uploads/logos", cancellationToken)
        };

        var result = await _restaurantService.UpdateBrandingAsync(_currentUserService.UserId!.Value, viewModel.RestaurantId, request, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.PaletteOptions = GetPalettes();
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        TempData["Success"] = "Branding updated.";
        return RedirectToAction(nameof(Branding), new { restaurantId = viewModel.RestaurantId });
    }

    private static Task<FileUploadRequest?> ToUploadAsync(IFormFile? formFile, string folder, CancellationToken cancellationToken)
    {
        return FormFileMapper.ToUploadAsync(formFile, folder, cancellationToken);
    }

    private static IReadOnlyList<BrandPaletteOptionViewModel> GetPalettes()
    {
        return BrandPaletteCatalog.All
            .Select(x => new BrandPaletteOptionViewModel
            {
                Key = x.Key,
                Name = x.Name,
                PrimaryColor = x.PrimaryColor,
                SecondaryColor = x.SecondaryColor
            })
            .ToList();
    }

    private static IReadOnlyList<RestaurantSwitcherViewModel> MapRestaurants(IReadOnlyList<RestaurantSummaryDto> restaurants)
    {
        return restaurants.Select(x => new RestaurantSwitcherViewModel
        {
            Id = x.Id,
            Name = x.Name,
            BranchName = x.BranchName,
            PublicMenuPath = x.PublicMenuPath
        }).ToList();
    }
}
