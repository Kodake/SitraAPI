using BackEnd.Entities.Finances;

namespace BackEnd.Entities.Products
{
    public sealed class ItemTaxes
    {
        public int ItemId { get; set; }
        public int TaxId { get; set; }
        public Item Item { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
    }
}