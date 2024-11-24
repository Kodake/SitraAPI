using BackEnd.Enums;

namespace BackEnd.Entities.Products
{
    public sealed class PricingItem
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public long PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public bool IsPrincipal { get; set; } = false;
    }
}
