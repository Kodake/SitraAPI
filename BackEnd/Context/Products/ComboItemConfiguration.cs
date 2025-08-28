using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Products
{
    public class ComboItemConfiguration : IEntityTypeConfiguration<ComboItem>
    {
        public void Configure(EntityTypeBuilder<ComboItem> builder)
        {
            builder.ToTable("ComboItems");

            builder.HasKey(ic => new { ic.ParentItemId, ic.ChildItemId });

            builder.HasOne(ic => ic.ParentItem)
                .WithMany(i => i.ComboItems)
                .HasForeignKey(ic => ic.ParentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ic => ic.ChildItem)
                .WithMany()
                .HasForeignKey(ic => ic.ChildItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
