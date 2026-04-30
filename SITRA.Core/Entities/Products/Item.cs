using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Item : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public ItemType ItemType { get; set; }
        public UnitMeasure? UnitMeasure { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public ItemBrand? ItemBrand { get; set; }
        public ItemModel? ItemModel { get; set; }
        public ItemCategory? ItemCategory { get; set; }
        public ItemSubCategory? ItemSubCategory { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = new List<ItemTaxes>();
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<PricingItem> PricingItems { get; set; } = new List<PricingItem>();
        public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
    }
}
