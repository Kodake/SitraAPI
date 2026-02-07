using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Products.Pricings
{
    public class PricingDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
