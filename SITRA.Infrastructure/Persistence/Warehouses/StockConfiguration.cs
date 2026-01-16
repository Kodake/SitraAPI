using SITRA.Core.Entities.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Warehouses;
public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.MinQty).HasColumnType("decimal(18,2)");
        builder.Property(p => p.MaxQty).HasColumnType("decimal(18,2)");
        builder
            .HasOne(ps => ps.Item)
            .WithMany(p => p.Stocks)
            .HasForeignKey(ps => ps.ItemId);
        builder
            .HasOne(ps => ps.Warehouse)
            .WithMany(w => w.Stocks)
            .HasForeignKey(ps => ps.WarehouseId);
    }
}
