using System.Text;
using System.Text.RegularExpressions;
using Menulo.Application.Abstractions.Persistence;
using Menulo.Application.Abstractions.Utilities;

namespace Menulo.Infrastructure.Services;

internal sealed class SlugService : ISlugService
{
    private readonly IRestaurantRepository _restaurantRepository;

    public SlugService(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<string> GenerateUniqueSlugAsync(string name, string? preferredSlug = null, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default)
    {
        var baseSlug = ToSlug(string.IsNullOrWhiteSpace(preferredSlug) ? name : preferredSlug);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = $"restaurant-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }

        var candidate = baseSlug;
        var suffix = 2;
        while (await _restaurantRepository.SlugExistsAsync(candidate, ignoreRestaurantId, cancellationToken))
        {
            candidate = $"{baseSlug}-{suffix++}";
        }

        return candidate;
    }

    private static string ToSlug(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", string.Empty);
        normalized = Regex.Replace(normalized, @"\s+", "-");
        normalized = Regex.Replace(normalized, @"-+", "-");
        return new string(normalized.Where(ch => char.IsLetterOrDigit(ch) || ch == '-').ToArray()).Trim('-');
    }
}
