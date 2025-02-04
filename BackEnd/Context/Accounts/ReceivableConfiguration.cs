using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Accounts
{
    public class ReceivableConfiguration : IEntityTypeConfiguration<Receivable>
    {
        public void Configure(EntityTypeBuilder<Receivable> builder)
        {
            builder.Property(p => p.TotalAmountDue).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalOutstanding).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(ar => ar.Invoice)
                .WithOne(s => s.Receivable)
                .HasForeignKey<Receivable>(ar => ar.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(ar => ar.PaymentReceipts)
                .WithOne(pr => pr.Receivable)
                .HasForeignKey(pr => pr.ReceivableId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}