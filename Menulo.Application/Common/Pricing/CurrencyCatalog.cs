using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Application.Common.Pricing;

public static class CurrencyCatalog
{
    public static IReadOnlyDictionary<CurrencyCode, string> Symbols { get; } = new Dictionary<CurrencyCode, string>
    {
        [CurrencyCode.INR] = "Rs",
        [CurrencyCode.USD] = "$",
        [CurrencyCode.EUR] = "EUR"
    };

    public static string GetSymbol(CurrencyCode code) => Symbols.TryGetValue(code, out var symbol) ? symbol : "Rs";
}
