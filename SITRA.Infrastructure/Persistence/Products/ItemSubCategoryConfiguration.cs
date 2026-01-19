using SITRA.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ItemSubCategoryConfiguration : IEntityTypeConfiguration<ItemSubCategory>
    {
        public void Configure(EntityTypeBuilder<ItemSubCategory> builder)
        {
            builder.HasKey(x => x.ItemId);
            builder.HasOne(x => x.Item)
                   .WithOne(x => x.ItemSubCategory)
                   .HasForeignKey<ItemSubCategory>(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.SubCategory)
                   .WithMany(x => x.ItemSubCategories)
                   .HasForeignKey(x => x.SubCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
