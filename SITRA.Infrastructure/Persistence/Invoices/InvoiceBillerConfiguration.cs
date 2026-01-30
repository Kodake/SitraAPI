using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class InvoiceBillerConfiguration : IEntityTypeConfiguration<InvoiceBiller>
    {
        public void Configure(EntityTypeBuilder<InvoiceBiller> builder)
        {
            builder.HasKey(x => x.InvoiceId);
            builder.HasOne(x => x.Invoice)
                   .WithOne(x => x.InvoiceBiller)
                   .HasForeignKey<InvoiceBiller>(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Biller)
                   .WithMany(x => x.InvoiceBillers)
                   .HasForeignKey(x => x.ContactId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
