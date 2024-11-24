using BackEnd.Entities.Finances;

namespace BackEnd.Entities.Products
{
    public sealed class ItemTaxes
    {
        public long ItemId { get; set; }
        public long TaxId { get; set; }
        public Item Item { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
    }
}