using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(p => p.SpotPayment)
                .WithMany(sp => sp.Payments)
                .HasForeignKey(p => p.SpotPaymentId)
                .IsRequired();
        }
    }
}
