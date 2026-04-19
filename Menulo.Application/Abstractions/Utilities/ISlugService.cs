namespace Menulo.Application.Abstractions.Utilities;

public interface ISlugService
{
    Task<string> GenerateUniqueSlugAsync(string name, string? preferredSlug = null, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default);
}
