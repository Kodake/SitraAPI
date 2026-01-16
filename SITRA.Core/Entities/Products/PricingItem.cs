using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class PricingItem : ITenant
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public long PricingId { get; set; }
        public virtual Pricing Pricing { get; set; } = null!;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public bool IsPrincipal { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
