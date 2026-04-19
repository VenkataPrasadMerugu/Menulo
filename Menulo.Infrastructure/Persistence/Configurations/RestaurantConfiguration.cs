using Menulo.Domain.Modules.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menulo.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("tbl_restaurants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.OwnerUserId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(120).IsRequired();
        builder.Property(x => x.BranchName).HasMaxLength(120);
        builder.Property(x => x.Address).HasMaxLength(240);
        builder.Property(x => x.LogoPath).HasMaxLength(512);
        builder.Property(x => x.PrimaryColor).HasMaxLength(7);
        builder.Property(x => x.SecondaryColor).HasMaxLength(7);
        builder.Property(x => x.PaletteKey).HasMaxLength(80);
        builder.Property(x => x.CurrencyCode).HasConversion<string>().HasMaxLength(8).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.OwnerUserId);
    }
}
