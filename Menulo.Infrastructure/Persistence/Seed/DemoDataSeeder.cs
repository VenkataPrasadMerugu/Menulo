using Menulo.Domain.Modules.Menu;
using Menulo.Domain.Modules.Restaurants;
using Menulo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Menulo.Infrastructure.Persistence.Seed;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(MenuloDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        await dbContext.Database.MigrateAsync();

        const string demoEmail = "owner@menulo.local";
        var owner = await userManager.Users.SingleOrDefaultAsync(x => x.Email == demoEmail);
        if (owner is null)
        {
            owner = new ApplicationUser
            {
                FullName = "Demo Owner",
                UserName = demoEmail,
                Email = demoEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(owner, "Demo@12345");
        }

        var restaurant = await dbContext.Restaurants
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.OwnerUserId == owner.Id);
        if (restaurant is null)
        {
            restaurant = Restaurant.Create(owner.Id, "Paradise Spice House", "paradise-spice-house", Domain.Modules.Restaurants.CurrencyCode.INR, "Banjara Hills", "Road No. 12, Hyderabad");
            restaurant.UpdateBranding("#8D1B3D", "#F2B544", "spice-house", "/uploads/logos/demo-logo.svg");
            await dbContext.Restaurants.AddAsync(restaurant);
            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.MenuItems.AnyAsync(x => x.RestaurantId == restaurant.Id))
        {
            return;
        }

        var items = new[]
        {
            MenuItem.Create(restaurant.Id, "Hyderabadi Dum Biryani", 349m, 2, MenuCategory.RiceAndBiryani, FoodType.NonVeg),
            MenuItem.Create(restaurant.Id, "Paneer Tikka Flatbread", 259m, 1, MenuCategory.Starters, FoodType.Veg),
            MenuItem.Create(restaurant.Id, "Mango Lassi", 129m, 1, MenuCategory.Beverages, FoodType.Veg)
        };

        items[1].SetStatus(MenuItemStatus.OutOfStock);
        items[2].SetStatus(MenuItemStatus.Inactive);

        await dbContext.MenuItems.AddRangeAsync(items);
        await dbContext.SaveChangesAsync();

        items[0].ReplaceImages(["/uploads/menu-items/demo-biryani.svg", "/uploads/menu-items/demo-biryani.svg"]);
        items[1].ReplaceImages(["/uploads/menu-items/demo-flatbread.svg"]);
        items[2].ReplaceImages(["/uploads/menu-items/demo-lassi.svg"]);
        await dbContext.SaveChangesAsync();
    }
}
