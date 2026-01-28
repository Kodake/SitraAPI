using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Finances;

namespace SITRA.Core.Entities.Products
{
    public sealed class ItemTaxes : ITenant, IAuditableEntity
    {
        public Guid ItemId { get; set; }
        public Guid TaxId { get; set; }
        public Item Item { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
