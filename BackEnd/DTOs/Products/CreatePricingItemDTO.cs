namespace BackEnd.DTOs.Products
{
    public class CreatePricingItemDTO
    {
        public int ItemId { get; set; }
        public int PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
    }
}
