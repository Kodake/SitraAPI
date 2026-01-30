using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ItemBrandConfiguration : IEntityTypeConfiguration<ItemBrand>
    {
        public void Configure(EntityTypeBuilder<ItemBrand> builder)
        {
            builder.HasKey(x => x.ItemId);
            builder.HasOne(x => x.Item)
                   .WithOne(x => x.ItemBrand)
                   .HasForeignKey<ItemBrand>(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Brand)
                   .WithMany(x => x.ItemBrands)
                   .HasForeignKey(x => x.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
