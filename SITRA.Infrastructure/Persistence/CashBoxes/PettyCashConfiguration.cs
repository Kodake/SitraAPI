using SITRA.Core.Entities.CashBoxes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.CashBoxes
{
    public class PettyCashConfiguration : IEntityTypeConfiguration<PettyCash>
    {
        public void Configure(EntityTypeBuilder<PettyCash> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.Property(p => p.OpeningAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.OpeningDate)
                   .IsRequired();

            builder.Property(p => p.ClosingAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired(false);

            builder.Property(p => p.ClosingDate)
                   .IsRequired(false);

            builder.Property(p => p.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(p => p.Comments)
                   .HasMaxLength(100);

            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PettyCashMovements)
                   .WithOne(m => m.PettyCash)
                   .HasForeignKey(m => m.PettyCashId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
