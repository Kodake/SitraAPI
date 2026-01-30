using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalTaxes).HasColumnType("decimal(18,2)").IsRequired();
            builder
                .HasOne(sii => sii.Invoice)
                .WithMany(w => w.InvoiceItems)
                .HasForeignKey(t => t.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
