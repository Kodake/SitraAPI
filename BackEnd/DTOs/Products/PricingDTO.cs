using BackEnd.Enums;

namespace BackEnd.DTOs.Products
{
    public class PricingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
    }
}
