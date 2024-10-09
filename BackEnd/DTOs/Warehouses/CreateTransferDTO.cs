using BackEnd.Enums;

namespace BackEnd.DTOs.Warehouses
{
    public class CreateTransferDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OriginWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public double Qty { get; set; }
        public int StockId { get; set; }
        public TransferStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
