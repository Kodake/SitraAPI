using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Context.Invoices
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
