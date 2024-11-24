namespace BackEnd.DTOs.Products
{
    public class CreatePricingItemDTO
    {
        public long ItemId { get; set; }
        public long PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
    }
}
