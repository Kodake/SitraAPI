using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products;
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(p => p.ItemType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Name).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Reference).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.Barcode).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Property(p => p.ImageUrl).IsUnicode().IsRequired(false);
        builder.Property(p => p.UnitMeasure).HasConversion<int>().IsRequired(false);

        builder.HasOne(i => i.Brand).WithMany().HasForeignKey(i => i.BrandId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(i => i.Model).WithMany().HasForeignKey(i => i.ModelId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(i => i.Category).WithMany().HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(i => i.SubCategory).WithMany().HasForeignKey(i => i.SubCategoryId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.TenantId);
    }
}
