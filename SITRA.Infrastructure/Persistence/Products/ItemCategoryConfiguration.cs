using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
    {
        public void Configure(EntityTypeBuilder<ItemCategory> builder)
        {
            builder.HasKey(x => x.ItemId);
            builder.HasOne(x => x.Item)
                   .WithOne(x => x.ItemCategory)
                   .HasForeignKey<ItemCategory>(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Category)
                   .WithMany(x => x.ItemCategories)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
