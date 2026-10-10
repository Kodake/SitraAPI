using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Products.Pricings
{
    public class CreatePricingDTO
    {
        public string Name { get; set; } = string.Empty;
        public PricingType PricingType { get; set; }
        public double? Percentage { get; set; }
    }
}
