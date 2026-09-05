namespace SITRA.Core.DTOs.Products.PricingItems
{
    public class PricingItemDTO
    {
        public Guid ItemId { get; set; }
        public Guid PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double? Percentage { get; set; }
        public bool IsPrincipal { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
