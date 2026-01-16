using SITRA.Core.Entities.Products;

namespace SITRA.Core.Entities.Warehouses
{
    public sealed class Stock
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
