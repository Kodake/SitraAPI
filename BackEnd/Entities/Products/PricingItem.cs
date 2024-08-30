namespace BackEnd.Entities.Products
{
    public sealed class PricingItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public int PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double Amount { get; set; } // Valor o porcentaje dependiendo del tipo en Pricing
        public bool IsPrincipal { get; set; } = false;

        public double FinalPrice
        {
            get
            {
                if (Pricing.PricingType == PricingType.Percentage)
                {
                    return BasePrice - (BasePrice * Amount / 100);
                }
                return BasePrice - Amount;
            }
        }
    }
}
