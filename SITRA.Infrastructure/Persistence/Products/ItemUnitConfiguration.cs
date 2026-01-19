using SITRA.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ItemUnitConfiguration : IEntityTypeConfiguration<ItemUnit>
    {
        public void Configure(EntityTypeBuilder<ItemUnit> builder)
        {
            builder.HasKey(x => x.ItemId);
            builder.HasOne(x => x.Item)
                   .WithOne(x => x.ItemUnit)
                   .HasForeignKey<ItemUnit>(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Unit)
                   .WithMany(x => x.ItemUnits)
                   .HasForeignKey(x => x.UnitId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
