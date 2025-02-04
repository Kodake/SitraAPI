using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Context.Invoices
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
