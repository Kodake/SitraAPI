using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.Infrastructure.Persistence.Warehouses
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Address).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Phone).HasMaxLength(10).IsRequired(false);
            builder.Property(p => p.Email).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.City).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.Country).HasMaxLength(2).IsRequired(false);
            builder.Property(p => p.ZipCode).HasMaxLength(5).IsRequired(false);
            builder.HasIndex(x => x.TenantId);
        }
    }
}
