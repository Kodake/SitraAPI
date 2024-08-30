namespace BackEnd.Entities.Products
{
    public sealed class Pricing
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; } = PricingType.Value;
        public double? Percentage { get; set; }
        public List<PricingItem> PricingItems { get; set; } = new List<PricingItem>();
    }
}
