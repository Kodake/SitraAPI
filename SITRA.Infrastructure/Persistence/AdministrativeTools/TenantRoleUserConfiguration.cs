using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Infrastructure.Persistence.AdministrativeTools
{
    public class TenantRoleUserConfiguration : IEntityTypeConfiguration<TenantRoleUser>
    {
        public void Configure(EntityTypeBuilder<TenantRoleUser> builder)
        {
            builder.ToTable("TenantRoleUsers", "dbo");

            builder.HasKey(tru => new { tru.RoleId, tru.UserId });

            builder.HasOne(tru => tru.Role)
                .WithMany(r => r.TenantRoleUsers)
                .HasForeignKey(tru => tru.RoleId);

            builder.HasOne(tru => tru.User)
                .WithMany()
                .HasForeignKey(tru => tru.UserId);
        }
    }
}
