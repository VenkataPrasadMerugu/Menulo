namespace Menulo.Web.ViewModels.Owner;

public sealed class RestaurantSwitcherViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string PublicMenuPath { get; set; } = string.Empty;
}
