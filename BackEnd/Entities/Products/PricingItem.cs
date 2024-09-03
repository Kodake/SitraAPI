using BackEnd.Enums;

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
        public bool IsPrincipal { get; set; } = false;
    }
}
