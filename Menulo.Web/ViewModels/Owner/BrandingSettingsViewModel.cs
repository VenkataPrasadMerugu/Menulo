using System.ComponentModel.DataAnnotations;

namespace Menulo.Web.ViewModels.Owner;

public sealed class BrandingSettingsViewModel
{
    public int RestaurantId { get; set; }

    [Display(Name = "Color palette")]
    public string? PaletteKey { get; set; } = "rose-cafe";

    [RegularExpression("^#(?:[0-9a-fA-F]{6})$")]
    [Display(Name = "Primary color")]
    public string PrimaryColor { get; set; } = "#A63A50";

    [RegularExpression("^#(?:[0-9a-fA-F]{6})$")]
    [Display(Name = "Secondary color")]
    public string SecondaryColor { get; set; } = "#F4B860";

    [Display(Name = "Logo")]
    public IFormFile? Logo { get; set; }

    public string PrimaryColorText { get; set; } = "#A63A50";
    public string SecondaryColorText { get; set; } = "#F4B860";
    public string? ExistingLogoPath { get; set; }
    public string? RestaurantSlug { get; set; }
    public IReadOnlyList<RestaurantSwitcherViewModel> Restaurants { get; set; } = [];
    public IReadOnlyList<BrandPaletteOptionViewModel> PaletteOptions { get; set; } = [];
}
