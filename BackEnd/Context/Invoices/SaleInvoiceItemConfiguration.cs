using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Invoices
{
    public class SaleInvoiceItemConfiguration : IEntityTypeConfiguration<SaleInvoiceItem>
    {
        public void Configure(EntityTypeBuilder<SaleInvoiceItem> builder)
        {
            builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalTaxes).HasColumnType("decimal(18,2)").IsRequired();
            builder
                .HasOne(sii => sii.SaleInvoice)
                .WithMany(w => w.SaleInvoiceItems)
                .HasForeignKey(t => t.SaleInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
