namespace BackEnd.DTOs.Products
{
    public class PricingItemDTO
    {
        public int ItemId { get; set; }
        public int PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double? Percentage { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
