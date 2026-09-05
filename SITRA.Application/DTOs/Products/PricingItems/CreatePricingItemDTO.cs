namespace SITRA.Core.DTOs.Products.PricingItems
{
    public class CreatePricingItemDTO
    {
        public Guid ItemId { get; set; }
        public Guid PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
    }
}
