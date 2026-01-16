using SITRA.Core.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
    {
        public void Configure(EntityTypeBuilder<Quotation> builder)
        {
            builder.HasOne(ar => ar.Invoice)
                .WithOne(s => s.Quotation)
                .HasForeignKey<Quotation>(ar => ar.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
