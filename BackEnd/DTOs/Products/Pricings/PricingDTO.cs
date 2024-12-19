using BackEnd.Enums;

namespace BackEnd.DTOs.Products.Pricings
{
    public class PricingDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
    }
}
