using BackEnd.Enums;

namespace BackEnd.DTOs.Products
{
    public class CreatePricingDTO
    {
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
    }
}
