using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class DeadlineConfiguration : IEntityTypeConfiguration<Deadline>
    {
        public void Configure(EntityTypeBuilder<Deadline> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Days).IsRequired();
            builder.HasIndex(x => x.TenantId);
        }
    }
}
