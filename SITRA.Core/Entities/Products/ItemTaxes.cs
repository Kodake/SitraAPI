using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Finances;

namespace SITRA.Core.Entities.Products
{
    public sealed class ItemTaxes : ITenant
    {
        public long ItemId { get; set; }
        public long TaxId { get; set; }
        public Item Item { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
