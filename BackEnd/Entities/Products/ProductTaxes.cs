using BackEnd.Entities.Finances;

namespace BackEnd.Entities.Products
{
    public sealed class ProductTaxes
    {
        public int ProductId { get; set; }
        public int TaxId { get; set; }
        public Product Product { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
    }
}