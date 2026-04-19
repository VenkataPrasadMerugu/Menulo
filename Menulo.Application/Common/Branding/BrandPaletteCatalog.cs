namespace Menulo.Application.Common.Branding;

public static class BrandPaletteCatalog
{
    public static readonly IReadOnlyList<BrandPalette> All =
    [
        new("spice-house", "Spice House", "#8D1B3D", "#F2B544"),
        new("leaf-garden", "Leaf Garden", "#216E39", "#D8F06A"),
        new("ocean-breeze", "Ocean Breeze", "#145DA0", "#7ED6DF"),
        new("sunset-grill", "Sunset Grill", "#D1495B", "#EDA65A"),
        new("midnight-lounge", "Midnight Lounge", "#1F2A44", "#C9A227"),
        new("rose-cafe", "Rose Cafe", "#A63A50", "#F4B860")
    ];

    public static BrandPalette Resolve(string? key)
    {
        return All.FirstOrDefault(x => x.Key == key) ?? All[^1];
    }
}
