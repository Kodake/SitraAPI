using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("TenantRoles", "dbo");
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();

            builder.Property(r => r.Description).HasMaxLength(100).IsRequired();

            builder.Property(r => r.IsSystemRole).HasDefaultValue(false).IsRequired();

            builder.HasIndex(r => new { r.TenantId, r.Name })
                .IsUnique()
                .HasDatabaseName("IX_Roles_TenantId_Name");
        }
    }
}
