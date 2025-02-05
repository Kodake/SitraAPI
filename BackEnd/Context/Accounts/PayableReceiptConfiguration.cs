using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Accounts
{
    public class PayableReceiptConfiguration : IEntityTypeConfiguration<PayableReceipt>
    {
        public void Configure(EntityTypeBuilder<PayableReceipt> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
