using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products;
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(50);
        builder.Property(p => p.ImageURL).IsRequired(false);
    }
}
