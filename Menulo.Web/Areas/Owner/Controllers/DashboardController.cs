using Menulo.Application.Abstractions.CurrentUser;
using Menulo.Application.DTOs.Restaurants;
using Menulo.Application.Services;
using Menulo.Domain.Modules.Menu;
using Menulo.Web.Infrastructure;
using Menulo.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Text;

namespace Menulo.Web.Areas.Owner.Controllers;

[Area("Owner")]
[Authorize]
[Route("owner/dashboard")]
public sealed class DashboardController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRestaurantService _restaurantService;

    public DashboardController(ICurrentUserService currentUserService, IRestaurantService restaurantService)
    {
        _currentUserService = currentUserService;
        _restaurantService = restaurantService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? restaurantId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        if (restaurantId.HasValue)
        {
            HttpContext.Session.SetSelectedRestaurantId(restaurantId.Value);
            return RedirectToAction(nameof(Index));
        }

        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(userId, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        if (dashboard.RestaurantId.HasValue)
        {
            HttpContext.Session.SetSelectedRestaurantId(dashboard.RestaurantId.Value);
        }

        return View(new DashboardViewModel
        {
            RestaurantName = dashboard.RestaurantName,
            RestaurantSlug = dashboard.RestaurantSlug,
            RestaurantId = dashboard.RestaurantId,
            BranchName = dashboard.BranchName,
            Address = dashboard.Address,
            PublicMenuUrl = dashboard.PublicMenuUrl,
            QrDownloadUrl = dashboard.RestaurantId.HasValue ? $"/owner/dashboard/menu-qr?restaurantId={dashboard.RestaurantId.Value}" : null,
            LogoPath = dashboard.LogoPath,
            PrimaryColor = dashboard.PrimaryColor,
            SecondaryColor = dashboard.SecondaryColor,
            CurrencySymbol = dashboard.CurrencySymbol,
            TotalMenuItems = dashboard.TotalMenuItems,
            ActiveItemCount = dashboard.ActiveItemCount,
            OutOfStockItemCount = dashboard.OutOfStockItemCount,
            HasRestaurantProfile = dashboard.HasRestaurantProfile,
            PreviewItems = dashboard.PreviewItems.Select(item => new DashboardPreviewItemViewModel
            {
                Name = item.Name,
                CategoryLabel = FormatCategory(item.Category),
                FoodTypeLabel = item.FoodType == FoodType.Veg ? "Veg" : "Non-Veg",
                Serves = item.Serves,
                Price = $"{dashboard.CurrencySymbol} {item.Price:0.##}",
                ImagePath = item.PrimaryImagePath,
                IsOutOfStock = item.Status == MenuItemStatus.OutOfStock
            }).ToList(),
            Restaurants = dashboard.Restaurants.Select(MapRestaurant).ToList()
        });
    }

    [HttpGet("menu-qr")]
    public async Task<IActionResult> MenuQr(int? restaurantId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        restaurantId ??= HttpContext.Session.GetSelectedRestaurantId();
        var dashboard = await _restaurantService.GetDashboardAsync(userId, $"{Request.Scheme}://{Request.Host}", restaurantId, cancellationToken);
        if (string.IsNullOrWhiteSpace(dashboard.PublicMenuUrl))
        {
            return NotFound();
        }

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(dashboard.PublicMenuUrl, QRCodeGenerator.ECCLevel.Q);
        var svg = new SvgQRCode(qrData).GetGraphic(12);
        return File(Encoding.UTF8.GetBytes(svg), "image/svg+xml", "menulo-menu-qr.svg");
    }

    [HttpGet("select/{restaurantId:int}")]
    public IActionResult SelectRestaurant(int restaurantId, string? returnUrl = null)
    {
        HttpContext.Session.SetSelectedRestaurantId(restaurantId);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    private static RestaurantSwitcherViewModel MapRestaurant(RestaurantSummaryDto restaurant)
    {
        return new RestaurantSwitcherViewModel
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            BranchName = restaurant.BranchName,
            PublicMenuPath = restaurant.PublicMenuPath
        };
    }

    private static string FormatCategory(MenuCategory category)
    {
        return category switch
        {
            MenuCategory.MainCourse => "Main Course",
            MenuCategory.RiceAndBiryani => "Rice & Biryani",
            _ => category.ToString()
        };
    }
}
