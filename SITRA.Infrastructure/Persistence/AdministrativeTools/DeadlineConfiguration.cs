using SITRA.Core.Entities.AdministrativeTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class DeadlineConfiguration : IEntityTypeConfiguration<Deadline>
    {
        public void Configure(EntityTypeBuilder<Deadline> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Days).IsRequired();
        }
    }
}
