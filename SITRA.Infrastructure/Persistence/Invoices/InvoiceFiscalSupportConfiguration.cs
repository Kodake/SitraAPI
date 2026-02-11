using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class InvoiceFiscalSupportConfiguration : IEntityTypeConfiguration<InvoiceFiscalSupport>
    {
        public void Configure(EntityTypeBuilder<InvoiceFiscalSupport> builder)
        {
            builder.HasKey(x => x.InvoiceId);
            builder.HasOne(x => x.Invoice)
                   .WithOne(x => x.InvoiceFiscalSupport)
                   .HasForeignKey<InvoiceFiscalSupport>(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.FiscalSupport)
                   .WithMany()
                   .HasForeignKey(x => x.FiscalSupportId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => x.TenantId);
        }
    }
}
