using Menulo.Application.Abstractions.Authentication;
using Menulo.Application.Abstractions.CurrentUser;
using Menulo.Application.Abstractions.Persistence;
using Menulo.Application.Abstractions.Storage;
using Menulo.Application.Abstractions.Utilities;
using Menulo.Application.Services;
using Menulo.Infrastructure.Identity;
using Menulo.Infrastructure.Persistence;
using Menulo.Infrastructure.Persistence.Repositories;
using Menulo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Menulo.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");

        services.AddDbContext<MenuloDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("tbl_migrations_history")));

        services
            .AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MenuloDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "Menulo.Auth.v2";
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/account/login";
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IMenuItemService, MenuItemService>();

        return services;
    }
}
