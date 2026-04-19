using Menulo.Domain.Modules.Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menulo.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemImageConfiguration : IEntityTypeConfiguration<MenuItemImage>
{
    public void Configure(EntityTypeBuilder<MenuItemImage> builder)
    {
        builder.ToTable("tbl_menu_item_images");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ImagePath).HasMaxLength(512).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.MenuItemId, x.SortOrder });
    }
}
