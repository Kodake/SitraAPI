using SITRA.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(p => p.Unit)
            .WithMany(u => u.Items)
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Brand)
            .WithMany(u => u.Items)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
