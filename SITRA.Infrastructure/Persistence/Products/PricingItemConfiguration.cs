using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products
{
    public class PricingItemConfiguration : IEntityTypeConfiguration<PricingItem>
    {
        public void Configure(EntityTypeBuilder<PricingItem> builder)
        {
            builder.Property(p => p.InitialCost).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
