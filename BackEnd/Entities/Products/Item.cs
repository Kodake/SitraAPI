using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.Entities.Products
{
    public sealed class Item
    {
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public double? InitialCost { get; set; } // Required for ItemType.Product
        public double BasePrice { get; set; }
        public double TotalPrice
        {
            get
            {
                var principalPricingItem = PricingItems.FirstOrDefault(pi => pi.IsPrincipal);
                if (principalPricingItem != null)
                {
                    double totalTaxes = ItemTaxes.Sum(pt => pt.Tax.Amount);
                    return principalPricingItem.FinalPrice + (principalPricingItem.FinalPrice * totalTaxes / 100);
                }
                return BasePrice + (BasePrice * ItemTaxes.Sum(pt => pt.Tax.Amount) / 100);
            }
        }
        public int? BrandId { get; set; } = null!;
        public Brand? Brand { get; set; }
        public int? CategoryId { get; set; } = null!;
        public Category? Category { get; set; }
        public int? UnitId { get; set; } = null!;
        public Unit? Unit { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
        public ICollection<Stock> Stocks { get; set; } = [];
        public ICollection<PricingItem> PricingItems { get; set; } = [];
    }
}
