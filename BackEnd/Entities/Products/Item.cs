using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.Entities.Products
{
    public sealed class Item
    {
        public long Id { get; set; }
        public ItemType ItemType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? BrandId { get; set; } = null!;
        public Brand? Brand { get; set; }
        public long? CategoryId { get; set; } = null!;
        public Category? Category { get; set; }
        public long? UnitId { get; set; } = null!;
        public Unit? Unit { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
        public ICollection<Stock> Stocks { get; set; } = [];
        public ICollection<PricingItem> PricingItems { get; set; } = [];
        public ICollection<Transfer> Transfers { get; set; } = [];
    }
}
