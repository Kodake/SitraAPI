using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Entities.Warehouses
{
    public class Transfer
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Item? Product { get; set; } = null!;
        private int originWarehouseId;
        public int OriginWarehouseId
        {
            get
            {
                return Stock?.WarehouseId ?? originWarehouseId;
            }
            set
            {
                originWarehouseId = value;
            }
        }
        public int DestinationWarehouseId { get; set; }
        public Warehouse? DestinationWarehouse { get; set; } = null!;
        public double Qty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int StockId { get; set; }
        public Stock? Stock { get; set; }
        public string Comment { get; set; } = string.Empty;
        public TransferStatus Status { get; set; }
    }
}
