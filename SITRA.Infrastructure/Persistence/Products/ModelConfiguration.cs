using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Products;
public class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.HasOne(p => p.Brand)
            .WithMany(c => c.Models)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(p => p.Name).HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(100).IsRequired(false);
    }
}
