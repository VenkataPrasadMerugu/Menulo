using System.ComponentModel.DataAnnotations;
using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Web.ViewModels.Owner;

public sealed class RestaurantProfileViewModel
{
    public int? RestaurantId { get; set; }

    [Required]
    [Display(Name = "Restaurant name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Branch name")]
    public string? BranchName { get; set; }

    [Display(Name = "Address")]
    public string? Address { get; set; }

    [Display(Name = "Restaurant logo")]
    public IFormFile? Logo { get; set; }

    [Display(Name = "Currency")]
    public CurrencyCode CurrencyCode { get; set; } = CurrencyCode.INR;

    [Display(Name = "Import menu from branch")]
    public int? ImportSourceRestaurantId { get; set; }

    public bool IsCreateMode { get; set; }
    public string? ExistingLogoPath { get; set; }
    public string? PublicMenuPath { get; set; }
    public IReadOnlyList<RestaurantSwitcherViewModel> Restaurants { get; set; } = [];
    public IReadOnlyList<RestaurantSwitcherViewModel> ImportSources { get; set; } = [];
}
