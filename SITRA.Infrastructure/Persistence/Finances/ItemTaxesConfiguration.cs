using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SITRA.Core.Entities.Products;

namespace SITRA.Infrastructure.Persistence.Finances
{
    public class ItemTaxesConfiguration : IEntityTypeConfiguration<ItemTaxes>
    {
        public void Configure(EntityTypeBuilder<ItemTaxes> builder)
        {
            builder.HasKey(g => new { g.TaxId, g.ItemId });
            builder.HasIndex(x => x.TenantId);
        }
    }
}
