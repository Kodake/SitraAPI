using BackEnd.Enums;

namespace BackEnd.DTOs.Warehouses
{
    public class TransferDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
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
        public WarehouseDTO? DestinationWarehouse { get; set; }
        public double Qty { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int StockId { get; set; }
        public StockDTO? Stock { get; set; }
        public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
