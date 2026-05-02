using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(p => p.Shipping).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(p => p.Discount).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(p => p.TotalQtyItems).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalItems).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalTaxes).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.DgiiTrackId).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.DgiiStatus).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.SecurityCode).HasMaxLength(250).IsRequired(false);

            builder
                .HasOne(s => s.Receivable)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Receivable>(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(s => s.SpotPayment)
                .WithOne(r => r.Invoice)
                .HasForeignKey<SpotPayment>(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(s => s.Quotation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Quotation>(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => x.TenantId);
        }
    }
}
