using SITRA.Core.DTOs.Warehouses.Warehouses;

namespace SITRA.Core.DTOs.Warehouses.Stocks
{
    public class StockDTO
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public ProductDTO Product { get; set; } = null!;
        public long WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
