using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("TenantPermissions", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Key).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Section).HasMaxLength(100).IsRequired();

            builder.HasIndex(p => p.Key).IsUnique();
        }
    }
}
