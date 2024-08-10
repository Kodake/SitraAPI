using BackEnd.Entities.Currencies;
using System.Collections.Generic;
using System.Linq;

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
        public List<Taxes> Taxes { get; set; } = new();

        public double TotalPrice
        {
            get
            {
                double totalTaxes = Taxes.Sum(t => t.Amount);
                return BasePrice + (BasePrice * totalTaxes / 100);
            }
        }

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; } = new();
        public int? CategoryId { get; set; }
        public Category? Category { get; set; } = new();
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; } = new();
        public bool IsDeleted { get; set; }
    }
}
