using SITRA.Core.Entities.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Warehouses
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();

            builder
                .HasOne(t => t.Product)
                .WithMany(i => i.Transfers)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(t => t.DestinationWarehouse)
                .WithMany(w => w.DestinationTransfers)
                .HasForeignKey(t => t.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(t => t.OriginWarehouse)
                .WithMany(w => w.OriginTransfers)
                .HasForeignKey(t => t.OriginWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
