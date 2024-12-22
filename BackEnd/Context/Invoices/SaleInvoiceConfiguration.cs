using BackEnd.Entities.Accounts;
using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Invoices
{
    public class SaleInvoiceConfiguration : IEntityTypeConfiguration<SaleInvoice>
    {
        public void Configure(EntityTypeBuilder<SaleInvoice> builder)
        {
            builder.Property(p => p.Shipping).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(p => p.Discount).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(p => p.TotalQtyItems).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalItems).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.TotalTaxes).HasColumnType("decimal(18,2)").IsRequired();
            builder.Ignore(p => p.FiscalSupport);
            builder
                .HasOne(invoice => invoice.Biller)
                .WithMany(contact => contact.BillerInvoices)
                .HasForeignKey(invoice => invoice.BillerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(invoice => invoice.Buyer)
                .WithMany(contact => contact.BuyerInvoices)
                .HasForeignKey(invoice => invoice.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(s => s.Receivable)
                .WithOne(r => r.SaleInvoice)
                .HasForeignKey<Receivable>(r => r.SaleInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}