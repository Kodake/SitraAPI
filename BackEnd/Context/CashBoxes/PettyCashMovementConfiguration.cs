using BackEnd.Entities.CashBoxes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.CashBoxes
{
    public class PettyCashMovementConfiguration : IEntityTypeConfiguration<PettyCashMovement>
    {
        public void Configure(EntityTypeBuilder<PettyCashMovement> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Amount)
                   .IsRequired();

            builder.Property(m => m.Type)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(m => m.CreatedAt)
                   .IsRequired();

            builder.HasOne(m => m.PettyCash)
                   .WithMany(p => p.PettyCashMovements)
                   .HasForeignKey(m => m.PettyCashId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Invoice)
                   .WithMany()
                   .HasForeignKey(m => m.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
