using Menulo.Domain.Common;

namespace Menulo.Domain.Modules.Restaurants;

public sealed class Restaurant : AuditableEntity
{
    private Restaurant()
    {
    }

    public int OwnerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? BranchName { get; private set; }
    public string? Address { get; private set; }
    public string? LogoPath { get; private set; }
    public string? PrimaryColor { get; private set; }
    public string? SecondaryColor { get; private set; }
    public string? PaletteKey { get; private set; }
    public CurrencyCode CurrencyCode { get; private set; } = CurrencyCode.INR;

    public static Restaurant Create(int ownerUserId, string name, string slug, CurrencyCode currencyCode, string? branchName = null, string? address = null)
    {
        return new Restaurant
        {
            OwnerUserId = ownerUserId,
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            CurrencyCode = currencyCode,
            BranchName = NormalizeOptional(branchName),
            Address = NormalizeOptional(address)
        };
    }

    public void UpdateProfile(string name, string slug, string? branchName = null, string? address = null, string? logoPath = null)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        BranchName = NormalizeOptional(branchName);
        Address = NormalizeOptional(address);

        if (!string.IsNullOrWhiteSpace(logoPath))
        {
            LogoPath = logoPath;
        }

        Touch();
    }

    public void UpdateBranding(string? primaryColor, string? secondaryColor, string? paletteKey, string? logoPath = null)
    {
        PrimaryColor = string.IsNullOrWhiteSpace(primaryColor) ? null : primaryColor.Trim();
        SecondaryColor = string.IsNullOrWhiteSpace(secondaryColor) ? null : secondaryColor.Trim();
        PaletteKey = string.IsNullOrWhiteSpace(paletteKey) ? null : paletteKey.Trim();

        if (!string.IsNullOrWhiteSpace(logoPath))
        {
            LogoPath = logoPath;
        }

        Touch();
    }

    public void UpdateCurrency(CurrencyCode currencyCode)
    {
        CurrencyCode = currencyCode;
        Touch();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
