using SITRA.Core.Interfaces;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Products
{
    public sealed class Pricing : ITenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public List<PricingItem> PricingItems { get; set; } = new List<PricingItem>();
    }
}
