using Microsoft.AspNetCore.Http;

namespace Menulo.Web.Infrastructure;

public static class RestaurantSelectionExtensions
{
    private const string SelectedRestaurantKey = "SelectedRestaurantId";

    public static int? GetSelectedRestaurantId(this ISession session)
    {
        return session.GetInt32(SelectedRestaurantKey);
    }

    public static void SetSelectedRestaurantId(this ISession session, int restaurantId)
    {
        session.SetInt32(SelectedRestaurantKey, restaurantId);
    }
}
