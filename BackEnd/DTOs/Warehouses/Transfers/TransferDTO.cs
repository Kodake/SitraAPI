using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Enums;

namespace BackEnd.DTOs.Warehouses.Transfers
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long StockId { get; set; }
        public StockDTO? Stock { get; set; }
        public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
