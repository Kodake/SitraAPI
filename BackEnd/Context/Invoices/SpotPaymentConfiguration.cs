using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Invoices
{
    public class SpotPaymentConfiguration : IEntityTypeConfiguration<SpotPayment>
    {
        public void Configure(EntityTypeBuilder<SpotPayment> builder)
        {
            builder.HasOne(ar => ar.Invoice)
                .WithOne(s => s.SpotPayment)
                .HasForeignKey<SpotPayment>(ar => ar.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(builder => builder.Payments);
            builder.Property(p => p.AmountReturned).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
