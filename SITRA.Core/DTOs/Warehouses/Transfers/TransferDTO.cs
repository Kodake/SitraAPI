using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Warehouses.Transfers
{
    public class TransferDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public long OriginWarehouseId { get; set; }
        public WarehouseDTO? OriginWarehouse { get; set; }
        public long DestinationWarehouseId { get; set; }
        public WarehouseDTO? DestinationWarehouse { get; set; }
        public double Qty { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public long StockId { get; set; }
        public StockDTO? Stock { get; set; }
        public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
