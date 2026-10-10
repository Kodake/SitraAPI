using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Finances;

namespace SITRA.Infrastructure.Persistence.Finances
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.Property(p => p.Code).HasMaxLength(20);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Symbol).HasMaxLength(3);
            builder.Property(p => p.Rate).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasIndex(x => x.TenantId);
        }
    }
}
