using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products;
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(p => p.ItemType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Reference).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.Barcode).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.Description).HasMaxLength(100).IsRequired(false);
        builder.Property(p => p.ImageUrl).IsUnicode().IsRequired(false);

    }
}
