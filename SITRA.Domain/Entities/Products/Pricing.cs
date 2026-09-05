using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Pricing : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public List<PricingItem> PricingItems { get; set; } = new List<PricingItem>();
    }
}
