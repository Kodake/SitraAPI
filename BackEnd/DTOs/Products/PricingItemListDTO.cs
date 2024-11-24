using BackEnd.Entities.Products;

namespace BackEnd.DTOs.Products
{
    public class PricingItemListDTO
    {
        public long Id { get; set; }
        public PricingDTO? Pricing { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double? Percentage { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
