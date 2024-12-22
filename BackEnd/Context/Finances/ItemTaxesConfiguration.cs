using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Context.Finances
{
    public class ItemTaxesConfiguration : IEntityTypeConfiguration<ItemTaxes>
    {
        public void Configure(EntityTypeBuilder<ItemTaxes> builder)
        {
            builder.HasKey(g => new { g.TaxId, g.ItemId });
        }
    }
}
