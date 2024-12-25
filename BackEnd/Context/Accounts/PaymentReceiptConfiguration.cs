using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Accounts
{
    public class PaymentReceiptConfiguration : IEntityTypeConfiguration<PaymentReceipt>
    {
        public void Configure(EntityTypeBuilder<PaymentReceipt> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
