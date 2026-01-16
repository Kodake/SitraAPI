using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Warehouses
{
    public class Transfer
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Item? Product { get; set; } = null!;
        public long OriginWarehouseId { get; set; }
        public Warehouse OriginWarehouse { get; set; } = null!;
        public long DestinationWarehouseId { get; set; }
        public Warehouse DestinationWarehouse { get; set; } = null!;
        public double Qty { get; set; }
        public long StockId { get; set; }
        public Stock? Stock { get; set; }
        public string Comment { get; set; } = string.Empty;
        public TransferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
