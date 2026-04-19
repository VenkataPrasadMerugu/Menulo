using Menulo.Domain.Modules.Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menulo.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("tbl_menu_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.RestaurantId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Price).HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.Serves).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.FoodType).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.RestaurantId, x.Status });
        builder.HasOne<Domain.Modules.Restaurants.Restaurant>()
            .WithMany()
            .HasForeignKey(x => x.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Images).AutoInclude();
        builder.Metadata.FindNavigation(nameof(MenuItem.Images))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
