using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Products;
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(20);
        builder.Property(p => p.ShortName).HasMaxLength(20);
    }
}
