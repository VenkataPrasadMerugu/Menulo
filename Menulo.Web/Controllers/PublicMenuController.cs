using Menulo.Application.Services;
using Menulo.Web.ViewModels.Public;
using Microsoft.AspNetCore.Mvc;

namespace Menulo.Web.Controllers;

public sealed class PublicMenuController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public PublicMenuController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet("/{restaurantSlug}/menu")]
    public async Task<IActionResult> Index(string restaurantSlug, CancellationToken cancellationToken)
    {
        var menu = await _restaurantService.GetPublicMenuAsync(restaurantSlug, cancellationToken);
        if (menu is null)
        {
            return NotFound();
        }

        return View(new PublicMenuViewModel
        {
            RestaurantId = menu.RestaurantId,
            RestaurantName = menu.RestaurantName,
            RestaurantSlug = menu.RestaurantSlug,
            BranchName = menu.BranchName,
            Address = menu.Address,
            LogoPath = menu.LogoPath,
            PrimaryColor = menu.PrimaryColor,
            SecondaryColor = menu.SecondaryColor,
            CurrencySymbol = menu.CurrencySymbol,
            Items = menu.Items
        });
    }
}
