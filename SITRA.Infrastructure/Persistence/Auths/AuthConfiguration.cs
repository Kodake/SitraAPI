using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Auths;

namespace SITRA.Infrastructure.Persistence.Auths
{
    public class AuthConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.CompanyName).HasMaxLength(100);
            builder.Property(p => p.RucNumber).HasMaxLength(11);
            builder.Property(p => p.Address).HasMaxLength(100);
            builder.Property(p => p.PhoneNumber).HasMaxLength(10);
            builder.Property(p => p.TenantId).IsRequired();
            builder.HasIndex(x => x.TenantId);
        }
    }
}
