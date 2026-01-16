using SITRA.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SITRA.Infrastructure.Persistence.Finances
{
    public class ItemTaxesConfiguration : IEntityTypeConfiguration<ItemTaxes>
    {
        public void Configure(EntityTypeBuilder<ItemTaxes> builder)
        {
            builder.HasKey(g => new { g.TaxId, g.ItemId });
        }
    }
}
