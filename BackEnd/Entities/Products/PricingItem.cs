namespace BackEnd.Entities.Products
{
    public class PricingItem
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public long PricingId { get; set; }
        public virtual Pricing Pricing { get; set; } = null!;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public bool IsPrincipal { get; set; } = false;
    }
}
