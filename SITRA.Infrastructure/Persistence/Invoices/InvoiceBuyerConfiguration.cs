using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class InvoiceBuyerConfiguration : IEntityTypeConfiguration<InvoiceBuyer>
    {
        public void Configure(EntityTypeBuilder<InvoiceBuyer> builder)
        {
            builder.HasKey(x => x.InvoiceId);
            builder.HasOne(x => x.Invoice)
                   .WithOne(x => x.InvoiceBuyer)
                   .HasForeignKey<InvoiceBuyer>(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Buyer)
                   .WithMany(x => x.InvoiceBuyers)
                   .HasForeignKey(x => x.ContactId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
