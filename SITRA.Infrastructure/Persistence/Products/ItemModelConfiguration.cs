using SITRA.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ItemModelConfiguration : IEntityTypeConfiguration<ItemModel>
    {
        public void Configure(EntityTypeBuilder<ItemModel> builder)
        {
            builder.HasKey(x => x.ItemId);
            builder.HasOne(x => x.Item)
                   .WithOne(x => x.ItemModel)
                   .HasForeignKey<ItemModel>(x => x.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Model)
                   .WithMany(x => x.ItemModels)
                   .HasForeignKey(x => x.ModelId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
