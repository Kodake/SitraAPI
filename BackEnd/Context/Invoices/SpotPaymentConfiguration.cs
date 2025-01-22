using BackEnd.Entities.Accounts;
using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Invoices
{
    public class SpotPaymentConfiguration : IEntityTypeConfiguration<SpotPayment>
    {
        public void Configure(EntityTypeBuilder<SpotPayment> builder)
        {
            builder.HasOne(ar => ar.SaleInvoice)
                .WithOne(s => s.SpotPayment)
                .HasForeignKey<SpotPayment>(ar => ar.SaleInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(p => p.AmountReceived).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.AmountReturned).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
