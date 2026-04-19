using Menulo.Application.Common.Results;
using Menulo.Application.DTOs.Restaurants;

namespace Menulo.Application.Services;

public interface IRestaurantService
{
    Task<DashboardDto> GetDashboardAsync(int ownerUserId, string baseUrl, int? restaurantId = null, CancellationToken cancellationToken = default);
    Task<RestaurantProfileDto?> GetOwnedRestaurantAsync(int ownerUserId, int? restaurantId = null, CancellationToken cancellationToken = default);
    Task<OperationResult> UpsertProfileAsync(int ownerUserId, RestaurantProfileRequest request, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateBrandingAsync(int ownerUserId, int restaurantId, BrandingSettingsRequest request, CancellationToken cancellationToken = default);
    Task<PublicMenuDto?> GetPublicMenuAsync(string restaurantSlug, CancellationToken cancellationToken = default);
    Task<bool> IsSlugAvailableAsync(string slug, int? ignoreRestaurantId = null, CancellationToken cancellationToken = default);
}
