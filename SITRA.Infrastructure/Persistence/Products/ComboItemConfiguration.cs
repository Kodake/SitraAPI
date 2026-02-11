using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class ComboItemConfiguration : IEntityTypeConfiguration<ComboItem>
    {
        public void Configure(EntityTypeBuilder<ComboItem> builder)
        {
            builder.ToTable("ComboItems");

            builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasKey(ic => new { ic.ParentItemId, ic.ChildItemId });

            builder.HasOne(ic => ic.ParentItem)
                .WithMany(i => i.ComboItems)
                .HasForeignKey(ic => ic.ParentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ic => ic.ChildItem)
                .WithMany()
                .HasForeignKey(ic => ic.ChildItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TenantId);
        }
    }
}
