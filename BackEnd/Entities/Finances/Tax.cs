using BackEnd.Entities.Products;

namespace BackEnd.Entities.Finances
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; }
        public List<ProductTaxes> ProductTaxes { get; set; } = new List<ProductTaxes>();
    }
}
