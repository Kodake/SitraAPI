using SITRA.Core.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Invoices
{
    public class SpotPayableConfiguration : IEntityTypeConfiguration<SpotPayable>
    {
        public void Configure(EntityTypeBuilder<SpotPayable> builder)
        {
            builder.HasOne(ar => ar.Invoice)
                .WithOne(s => s.SpotPayable)
                .HasForeignKey<SpotPayable>(ar => ar.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
