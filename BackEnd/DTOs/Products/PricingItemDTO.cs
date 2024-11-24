namespace BackEnd.DTOs.Products
{
    public class PricingItemDTO
    {
        public long ItemId { get; set; }
        public long PricingId { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double? Percentage { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
