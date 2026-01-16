using SITRA.Core.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Accounts
{
    public class PayableConfiguration : IEntityTypeConfiguration<Payable>
    {
        public void Configure(EntityTypeBuilder<Payable> builder)
        {
            builder.Property(p => p.TotalAmountDue).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalOutstanding).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(ar => ar.Invoice)
                .WithOne(s => s.Payable)
                .HasForeignKey<Payable>(ar => ar.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(ar => ar.PayableReceipts)
                .WithOne(pr => pr.Payable)
                .HasForeignKey(pr => pr.PayableId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
