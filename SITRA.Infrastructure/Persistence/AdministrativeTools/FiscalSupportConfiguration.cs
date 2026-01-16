using SITRA.Core.Entities.AdministrativeTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class FiscalSupportConfiguration : IEntityTypeConfiguration<FiscalSupport>
    {
        public void Configure(EntityTypeBuilder<FiscalSupport> builder)
        {
            builder.Property(p => p.DocumentType).HasConversion<int>().IsRequired();
            builder.Property(p => p.FiscalSupportType).HasConversion<int>().IsRequired();
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Symbol).HasMaxLength(1).IsRequired();
            builder.Property(p => p.InitialSequence).IsRequired();
            builder.Property(p => p.FinalSequence).IsRequired();
            builder.Property(p => p.SpaceNumber).IsRequired();
            builder.HasOne(p => p.Warehouse)
                .WithMany(c => c.FiscalSupports)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
