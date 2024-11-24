using BackEnd.Enums;

namespace BackEnd.Entities.Products
{
    public sealed class Pricing
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
        public List<PricingItem> PricingItems { get; set; } = new List<PricingItem>();
    }
}
