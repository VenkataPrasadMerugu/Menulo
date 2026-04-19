using Menulo.Domain.Common;
using Menulo.Domain.Modules.Menu;
using Menulo.Domain.Modules.Restaurants;
using Menulo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Menulo.Infrastructure.Persistence;

public sealed class MenuloDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public MenuloDbContext(DbContextOptions<MenuloDbContext> options)
        : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemImage> MenuItemImages => Set<MenuItemImage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("tbl_users");
        builder.Entity<IdentityRole<int>>().ToTable("tbl_roles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("tbl_user_claims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("tbl_user_logins");
        builder.Entity<IdentityUserRole<int>>().ToTable("tbl_user_roles");
        builder.Entity<IdentityUserToken<int>>().ToTable("tbl_user_tokens");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("tbl_role_claims");

        builder.ApplyConfigurationsFromAssembly(typeof(MenuloDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditableEntries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in auditableEntries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.Touch();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
