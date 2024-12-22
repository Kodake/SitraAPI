using BackEnd.Entities.Finances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Finances
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.Property(p => p.Code).HasMaxLength(20);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Rate).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}