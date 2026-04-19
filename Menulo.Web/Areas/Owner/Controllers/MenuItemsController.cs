using Menulo.Application.Abstractions.CurrentUser;
using Menulo.Application.DTOs.Menu;
using Menulo.Application.DTOs.Restaurants;
using Menulo.Application.Services;
using Menulo.Domain.Modules.Menu;
using Menulo.Web.Infrastructure;
using Menulo.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menulo.Web.Areas.Owner.Controllers;

[Area("Owner")]
[Authorize]
public sealed class MenuItemsController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMenuItemService _menuItemService;
    private readonly IRestaurantService _restaurantService;

    public MenuItemsController(ICurrentUserService currentUserService, IMenuItemService menuItemService, IRestaurantService restaurantService)
    {
        _currentUserService = currentUserService;
        _menuItemService = menuItemService;
        _restaurantService = restaurantService;
    }

    [HttpGet("/owner/menu-items")]
    public async Task<IActionResult> Index(int? restaurantId, int? importSourceRestaurantId, CancellationToken cancellationToken)
    {
        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        if (!dashboard.RestaurantId.HasValue)
        {
            return View(new MenuItemListViewModel());
        }
        HttpContext.Session.SetSelectedRestaurantId(dashboard.RestaurantId.Value);

        var items = await _menuItemService.GetOwnedItemsAsync(_currentUserService.UserId!.Value, dashboard.RestaurantId.Value, cancellationToken);
        var importableItems = new List<MenuItemImportOptionViewModel>();
        if (importSourceRestaurantId.HasValue && importSourceRestaurantId.Value != dashboard.RestaurantId.Value)
        {
            var sourceItems = await _menuItemService.GetOwnedItemsAsync(_currentUserService.UserId!.Value, importSourceRestaurantId.Value, cancellationToken);
            importableItems = sourceItems.Select(x => new MenuItemImportOptionViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.ToString(),
                FoodType = x.FoodType.ToString(),
                Price = x.Price,
                PrimaryImagePath = x.PrimaryImagePath
            }).ToList();
        }

        return View(new MenuItemListViewModel
        {
            RestaurantId = dashboard.RestaurantId.Value,
            Restaurants = MapRestaurants(dashboard.Restaurants),
            ImportSources = MapRestaurants(dashboard.Restaurants.Where(x => x.Id != dashboard.RestaurantId.Value).ToList()),
            ImportSourceRestaurantId = importSourceRestaurantId,
            ImportableItems = importableItems,
            Items = items
        });
    }

    [HttpGet("/owner/menu-items/create")]
    public async Task<IActionResult> Create(int? restaurantId, CancellationToken cancellationToken)
    {
        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        if (dashboard.RestaurantId.HasValue)
        {
            HttpContext.Session.SetSelectedRestaurantId(dashboard.RestaurantId.Value);
        }
        return View(new MenuItemFormViewModel
        {
            RestaurantId = dashboard.RestaurantId ?? 0,
            Restaurants = MapRestaurants(dashboard.Restaurants)
        });
    }

    [HttpPost("/owner/menu-items/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItemFormViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        var result = await _menuItemService.CreateAsync(
            _currentUserService.UserId!.Value,
            viewModel.RestaurantId,
            new MenuItemUpsertRequest
            {
                Name = viewModel.Name,
                Price = viewModel.Price,
                Serves = viewModel.Serves,
                Category = viewModel.Category,
                FoodType = viewModel.FoodType,
                ImageUploads = await FormFileMapper.ToUploadsAsync(viewModel.Images, "uploads/menu-items", cancellationToken)
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        TempData["Success"] = "Menu item created.";
        return RedirectToAction(nameof(Index), new { restaurantId = viewModel.RestaurantId });
    }

    [HttpGet("/owner/menu-items/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, int? restaurantId, CancellationToken cancellationToken)
    {
        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        if (!dashboard.RestaurantId.HasValue)
        {
            return RedirectToAction(nameof(Index));
        }
        HttpContext.Session.SetSelectedRestaurantId(dashboard.RestaurantId.Value);

        var item = await _menuItemService.GetOwnedItemAsync(_currentUserService.UserId!.Value, dashboard.RestaurantId.Value, id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return View(new MenuItemFormViewModel
        {
            Id = item.Id,
            RestaurantId = dashboard.RestaurantId.Value,
            Name = item.Name,
            Price = item.Price,
            Serves = item.Serves,
            Category = item.Category,
            FoodType = item.FoodType,
            ExistingImagePaths = item.ImagePaths,
            Restaurants = MapRestaurants(dashboard.Restaurants)
        });
    }

    [HttpPost("/owner/menu-items/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuItemFormViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        var result = await _menuItemService.UpdateAsync(
            _currentUserService.UserId!.Value,
            viewModel.RestaurantId,
            id,
            new MenuItemUpsertRequest
            {
                Name = viewModel.Name,
                Price = viewModel.Price,
                Serves = viewModel.Serves,
                Category = viewModel.Category,
                FoodType = viewModel.FoodType,
                ImageUploads = await FormFileMapper.ToUploadsAsync(viewModel.Images, "uploads/menu-items", cancellationToken)
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            var dashboard = await _restaurantService.GetDashboardAsync(_currentUserService.UserId!.Value, $"{Request.Scheme}://{Request.Host}", viewModel.RestaurantId, cancellationToken);
            viewModel.Restaurants = MapRestaurants(dashboard.Restaurants);
            return View(viewModel);
        }

        TempData["Success"] = "Menu item updated.";
        return RedirectToAction(nameof(Index), new { restaurantId = viewModel.RestaurantId });
    }

    [HttpPost("/owner/menu-items/import")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(int restaurantId, int importSourceRestaurantId, List<int> selectedMenuItemIds, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.ImportAsync(_currentUserService.UserId!.Value, restaurantId, importSourceRestaurantId, selectedMenuItemIds, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Menu imported successfully." : result.Error;
        return RedirectToAction(nameof(Index), new { restaurantId, importSourceRestaurantId });
    }

    [HttpPost("/owner/menu-items/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int restaurantId, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.DeleteAsync(_currentUserService.UserId!.Value, restaurantId, id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Menu item deleted." : result.Error;
        return RedirectToAction(nameof(Index), new { restaurantId });
    }

    [HttpPost("/owner/menu-items/{id:int}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, int restaurantId, MenuItemStatus status, CancellationToken cancellationToken)
    {
        var result = await _menuItemService.SetStatusAsync(_currentUserService.UserId!.Value, restaurantId, id, status, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? $"Status changed to {status}." : result.Error;
        return RedirectToAction(nameof(Index), new { restaurantId });
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
