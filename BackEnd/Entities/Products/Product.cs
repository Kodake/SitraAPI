using BackEnd.Entities.Currencies;

namespace BackEnd.Entities.Products
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public double TotalPrice
        {
            get
            {
                double totalTaxes = ProductTaxes.Sum(pt => pt.Tax.Amount);
                return BasePrice + (BasePrice * totalTaxes / 100);
            }
        }
        public int? BrandId { get; set; } = null!;
        public Brand? Brand { get; set; }
        public int? CategoryId { get; set; } = null!;
        public Category? Category { get; set; }
        public int? UnitId { get; set; } = null!;
        public Unit? Unit { get; set; }
        public bool IsActive { get; set; }
        public List<ProductTaxes> ProductTaxes { get; set; } = new List<ProductTaxes>();
    }
}
